

# Kernel for random number generator initialization
randString = """
            #include <pyopencl-ranluxcl.cl>
            
            __kernel void init_ranlux(unsigned int seeds,
            __global ranluxcl_state_t* ranluxcltab, unsigned int NItems)
            {
            if(get_global_id(0) < NItems)
            ranluxcl_initialization(seeds, ranluxcltab);
            
            }            
"""

# Generate OpenCL file
SSAStringInit = """
#include <pyopencl-ranluxcl.cl>


__kernel void run(__global float* tau, __global float* gSpecies, __global float* gReactions,
                    __global float* gObs, __global ranluxcl_state_t* ranluxcltab,
                    const float targetTau, const int offset, const int NRUNS)
{
    const int gid = get_global_id(0) + offset;
    const int grow = ${NSPECIES}*gid;
    const int greact = ${NREACTIONS}*get_global_id(0);
    const int gobs = ${NOBSERVATIONS}*gid;

    const int lid = get_local_id(0);
    const int lrow = ${NSPECIES}*lid;
    const int lobs = ${NOBSERVATIONS}*lid;
    const int lfunc = ${NFUNCS}*lid;

    float Species[${NSPECIES}];
    float Obs[${NOBSERVATIONS}];
    float Func[${NFUNCS}];


    // Only compute if we are hitting a valid target
    if (gid<NRUNS)
    {
        float time = tau[gid]; // Current time in thread
        

        
        // Copy local populations and reactions to shared memory
        for(int ii=0; ii<${NSPECIES}; ii++)
            Species[ii] = gSpecies[grow+ii];
        // ---------------------------------------------
        // Begin code for computing propensities
        float a0 = 0.0;
        % for cmd in cmdList:
            ${cmd}
        % endfor        
        // End code for computing propensities
        // ---------------------------------------------
        
        for(int ii=0; ii<${NOBSERVATIONS}; ii++)
            gObs[gobs+ii] = Obs[ii];
    }
    
}

"""            


SSARunStepString = """
#include <pyopencl-ranluxcl.cl>

__kernel void run(__global float* tau, __global float* gSpecies, __global float* gReactions,
                     __global float* gObs, __global ranluxcl_state_t* ranluxcltab,
                     const float targetTau, const int offset, const int NRUNS)
{
    
    
    const int gid = get_global_id(0) + offset;
    const int grow = ${NSPECIES}*gid;
    const int gobs = ${NOBSERVATIONS}*gid;
    const int greact = ${NREACTIONS}*get_global_id(0);

    const int lid = get_local_id(0);
    const int lrow = ${NSPECIES}*lid;
    const int lobs = ${NOBSERVATIONS}*lid;
    const int lfunc = ${NFUNCS}*lid;

    float Species[${NSPECIES}];
    float Obs[${NOBSERVATIONS}];
    float Func[${NFUNCS}];


    
    // Only compute if we are hitting a valid target
    if (gid<NRUNS)
    {
        

        
        // Copy time
        float time = tau[gid]; // Current time in thread
        float r2;
        
        // Random number vector
        float4 randval;
        int ind;
        
        // Copy  populations and reactions to shared memory
        for(int ii=0; ii<${NSPECIES}; ii++)
            Species[ii] = gSpecies[grow+ii];
        for(int ii=0; ii<${NOBSERVATIONS}; ii++)
            Obs[ii] =  gObs[gobs+ii];
        
        // Setup Random number generator
        ranluxcl_state_t ranluxclstate;
        ranluxcl_download_seed(&ranluxclstate, ranluxcltab);
        
        while(time<targetTau)
        {
        
            randval = ranluxcl32(&ranluxclstate); // Draw random number
            for(int draw=0; draw<2; draw++)
            {
            
                if(time<targetTau)
                {

                        float a0 = 0.0;
                        ind = 0;
                        
                        % for cmd in cmdList:
                            ${cmd}
                        %endfor
                
                        a0 = gReactions[greact+${NREACTIONS}-1];
                        time -= ((draw==0)*log(randval.x)+(draw==1)*log(randval.z))/a0;
                        r2 = ((draw==0)*randval.y+(draw==1)*randval.w)*a0;
                        
                        
                        for(int jj =0; jj<${NREACTIONS}; jj++)
                        {
                            if(r2>gReactions[greact+jj])
                                ind += 1;
                        }
                        
                    
                    // select ind from reactions
                    switch(ind)
                    {
                        ${changeString}
                    }        
                }
            
            }
        }
        
        // Write populations and times to global memory
        tau[gid] = time;
        
        for(int ii=0; ii<${NSPECIES}; ii++)
            gSpecies[grow+ii] = Species[ii];
        for(int ii=0; ii<${NOBSERVATIONS}; ii++)
            gObs[gobs+ii] = Obs[ii];
        
        // Upload random number generator seed
        ranluxcl_upload_seed(&ranluxclstate, ranluxcltab);
        
    }
}
"""

SSARunEventString = """
#include <pyopencl-ranluxcl.cl>

__kernel void run(__global float* tau, __global float* gSpecies, __global float* gReactions,
                     __global float* gObs, __global ranluxcl_state_t* ranluxcltab,
                     const float targetTau, const int offset, const int NRUNS)
{
    
    
    const int gid = get_global_id(0) + offset;
    const int grow = ${NSPECIES}*gid;
    const int gobs = ${NOBSERVATIONS}*gid;
    
    const int lid = get_local_id(0);
    const int lrow = ${NSPECIES}*lid;
    const int lobs = ${NOBSERVATIONS}*lid;
    const int lfunc = ${NFUNCS}*lid;
    
    //__local float Species[${NSPECIES}*${NLOCAL}];
    //__local float Obs[${NOBSERVATIONS}*${NLOCAL}];
    //__local float Func[${NFUNCS}*${NLOCAL}];
    float Species[${NSPECIES}];
    float Obs[${NOBSERVATIONS}];
    float Func[${NFUNCS}];
    
    // Only compute if we are hitting a valid target
    if (gid<NRUNS)
    {
        

        // Copy time
        float time = tau[gid]; // Current time in thread
        int ind = 0; // draw index
        float r2;
        
        // Random number vector
        float4 randval;
        
        // Copy local populations and reactions to shared memory
        for(int ii=0; ii<${NSPECIES}; ii++)
            Species[ii] = gSpecies[grow+ii];
        for(int ii=0; ii<${NOBSERVATIONS}; ii++)
            Obs[ii] =  gObs[gobs+ii];
        barrier(LOCAL_MEM_FENCE);
        
        // Setup Random number generator
        ranluxcl_state_t ranluxclstate;
        ranluxcl_download_seed(&ranluxclstate, ranluxcltab);
        
        while(time<targetTau)
        {
        
            randval = ranluxcl32(&ranluxclstate); // Draw random number
            for(int draw=0; draw<2; draw++)
            {
            
            float a0 = 0.0;
            ind = 0;
            
            % for cmd in cmdList:
                ${cmd}
            %endfor
    
            a0 = gReactions[greact+${NREACTIONS}-1];
            time -= ((draw==0)*log(randval.x)+(draw==1)*log(randval.z))/a0;
            r2 = ((draw==0)*randval.y+(draw==1)*randval.w)*a0;
            
            
            for(int jj =0; jj<${NREACTIONS}; jj++)
            {
                if(r2>gReactions[greact+jj])
                    ind += 1;
            }
            
            
            // select ind from reactions
            if(time<targetTau)
                switch(ind)
                {
                    ${changeString}
                }

            }
        }
        
        // Write populations and times to global memory
        tau[gid] = targetTau;
        
        for(int ii=0; ii<${NSPECIES}; ii++)
            gSpecies[grow+ii] = Species[ii];
        for(int ii=0; ii<${NOBSERVATIONS}; ii++)
            gObs[gobs+ii] = Obs[ii];
        barrier(LOCAL_MEM_FENCE);
        
        // Upload random number generator seed
        ranluxcl_upload_seed(&ranluxclstate, ranluxcltab);
        
    }
}
"""




