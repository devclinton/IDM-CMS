function [gamma, pc, n] = mergeData(num_in, denom_in, pc_in, n_in, outvec, update_index)
M = length(num_in);
gamma = cell(1,M);
pc = cell(1,M);
n = cell(1,M);

for i=1:M
    
    if ismember(i, update_index)
        ovi = outvec{i};
        ovilen = length(ovi);
        numi = zeros(1, ovilen);
        denomi = zeros(1, ovilen);
        ni = zeros(1, ovilen);
        pci = zeros(1,ovilen-1);
        
        for j=1:ovilen-1
            numi(j) = sum(num_in{i}(ovi{j}(1):ovi{j}(2)));
            denomi(j) = sum(denom_in{i}(ovi{j}(1):ovi{j}(2)));
            pci(j) = pc_in{i}(ovi{j}(2));
            ni(j) = sum(n_in{i}(ovi{j}(1):ovi{j}(2)));
        end
        numi(ovilen) = sum(num_in{i}(ovi{ovilen}(1):ovi{ovilen}(2)));
        denomi(ovilen) = sum(denom_in{i}(ovi{ovilen}(1):ovi{ovilen}(2)));
        gamma{i} = numi./denomi;
        ni(ovilen) = sum(n_in{i}(ovi{ovilen}(1):ovi{ovilen}(2)));
        n{i} = ni;
        
        if length(gamma{i})==1
            pci = 1;
        end
        pc{i} = pci;        
    else
        gamma{i} = 1;
        pc{i} = 1;
    end
end