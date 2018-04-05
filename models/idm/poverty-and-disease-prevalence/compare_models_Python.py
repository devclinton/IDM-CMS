""" Python code to compare deterministic and CMS models of the 
poverty trap system. """
from __future__ import print_function

## Standard stuff
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

## For integrating the deterministic system
from scipy.integrate import odeint

## For smoothing 2d phase plots
from scipy.interpolate import Rbf

## And for running CMS, we have some simple functions 
## in cms_interface.py
import cms_interface as cms

## Set the seed
np.random.seed(123)

### The determinisitic model
##############################################################################
def dxdt(x,t,*args):

	## unpack the state
	I, M = x

	## unpack the params:
	alpha, mu, nu, r, beta1, gamma1, epsilon, kappa, M0 = args

	## Compute coefficients
	beta = beta1*epsilon/(M+epsilon)
	gamma = gamma1*M/(M+kappa)

	## Evaluate the derivative
	dMdt = -r*M*(M-M0*(1.-I))
	dIdt = beta*I*(1.-I)-(gamma+nu+alpha)*I+nu*I*I

	return np.array([dIdt, dMdt])


def Trajectory(x0,t,params):
	trajectory = odeint(dxdt,x0,t,args=tuple(params))
	return trajectory

def Nullclines(params):

	## unpack the params:
	alpha, mu, nu, r, beta1, gamma1, epsilon, kappa, M0 = params

	## I*(M)
	M = np.linspace(0., M0, 100)
	beta = beta1*epsilon/(M+epsilon)
	gamma = gamma1*M/(M+kappa)
	Istar = np.maximum(np.zeros(M.shape), (beta - gamma - alpha - nu)/(beta-nu))

	## M*(I) next
	I = np.linspace(0., 1., 100)
	Mstar = M0*(1.-I)

	return M, Istar, I, Mstar

def PhasePortrait(params):

	## unpack the params:
	alpha, mu, nu, r, beta1, gamma1, epsilon, kappa, M0 = params

	## Set of integration times for the trajectories
	t = np.linspace(0.,10.,100)

	## Grid of I and M initial values. We can be really
	## dense here since integrating ODEs is cheap.
	M = np.linspace(0.,M0,100)
	I = np.linspace(0.,1.,100)
	ics = np.array([[i,m] for m in M for i in I])

	## Compute the trajectories and store
	## a 1.0 if the final value has low
	## prevalence.
	phase = np.zeros((ics.shape[0],))
	for i, ic in enumerate(ics):
		traj = Trajectory(ic,t,params)
		if traj[-1,0] < 0.5:
			phase[i] = 1.
		else:
			phase[i] = 0.
	phase = np.reshape(phase, (len(I),len(M)))

	return I, M, phase

def PlotTrajectory(axes, traj, alpha=0.6,markers=True):

	if traj[-1,0] >= 0.5:
		color = "C4"
	else:
		color = "C0"

	axes.plot(traj[:,0],traj[:,1],c=color,alpha=alpha)

	if markers:
		axes.plot([traj[0,0], traj[-1,0]], [traj[0,1], traj[-1,1]], 
			  	  c="k",marker="o",ls="None",alpha=alpha)
	return axes


### Parsing the output from CMS
##############################################################################
def ParseCMS(dfs):

	""" Function to convert a list of cms dfs into trajectory objects like those that
	come from the deterministic simulation. """

	trajs = []
	for df in dfs:
		for i, sf in df.groupby("trajectory"):

			## Drop the extraneous trajcetory column
			sf = sf.drop("trajectory",axis=1)

			## Trajectory number string
			s = "{"+str(i)+"}"

			## Compute the population fraction that's 
			## infected.
			sf.loc["I"] = sf.loc["infected"+s] / sf.loc["population"+s]

			## Convert to np array
			array = sf.loc[["I","income"+s]].as_matrix()

			## Store
			trajs.append(array.T)

	return trajs

### CMS phase portrait
##############################################################################
def CMSProbabilityPortrait(params):

	""" Function to approximate the probability of poverty trap escape as a function
	of initial (I,M) point. 

	NB: This function is pretty expensive."""

	## unpack the params:
	alpha, mu, nu, r, beta1, gamma1, epsilon, kappa, M0 = params

	## Calculation size
	grid_points = 12  # grid points in (I,M) space
	pop = 100        # total population in the CMS
	n = 10			 # number of CMS trajectories per IC

	## Construct the IC array
	M = np.linspace(0.,M0,grid_points)
	I = np.linspace(0.,1.,grid_points)
	ics = np.array([[i,m] for m in M for i in I])
	probability = np.zeros((ics.shape[0],))

	## Prepare the CMS simulations
	cms.CFG(num_runs=n)
	prows = ["population{"+str(i)+"}" for i in range(n)]
	irows = ["infected{"+str(i)+"}" for i in range(n)]

	## Start simulating
	for i, ic in enumerate(ics):

		print("Running CMS simulation {}/{}".format(i+1, len(ics)))
		
		## Constructe CMS ic
		I_tilde = int(ic[0]*pop)
		S_tilde = pop - I_tilde
		cms_ic = [S_tilde, I_tilde, ic[1]]

		## Run CMS
		cms.EMODL(cms_ic, params)
		cms.RunCMS(verbose=False)
		df = cms.GetTrajectories()
		df = df.drop("trajectory",axis=1)

		## Compute probability of development
		population = df.loc[prows].iloc[:,-1].as_matrix()
		infecteds = df.loc[irows].iloc[:,-1].as_matrix()
		fraction = 1. - infecteds/np.array(population,dtype=float)
		probability[i] = fraction.round().mean()

	## Reshape the probabilities
	probability = probability.reshape((len(I),len(M)))

	return I, M, probability

if __name__ == "__main__":

	## Calculate the CMS probability portrait (this is
	## a little expensive, so here's an option to do everything
	## but this).
	compute_prob_portrait = True

	## Parameters
	alpha = 0.06
	mu = 0.01
	nu = 0.02
	r = 0.05
	beta1 = 40.
	gamma1 = 0.15
	epsilon = 0.3
	kappa = 100.
	M0 = 100.
	params = np.array([alpha,mu,nu,r,beta1,gamma1,epsilon,kappa,M0])

	## Compute some sample trajectories
	t = np.linspace(0., 1000., 10000)
	ics = np.random.uniform(low=[0., 0.], high=[1., M0], size=(10,2))
	print("Computing sample deterministic trajectories...")
	trajs = [Trajectory(ic,t,params) for ic in ics]
	print("finished.")

	## Compute cms trajectories for the same ics
	cms.CFG(num_runs=1)
	cms_outputs = []
	print("Computing sample CMS trajectories...")
	for ic in ics:
		I_tilde = int(ic[0]*1000)
		S_tilde = 1000 - I_tilde
		cms.EMODL([S_tilde, I_tilde, ic[1]],params)
		cms.RunCMS(verbose=False)
		cms_outputs.append(cms.GetTrajectories())
	print("finished.")
	cms_trajs = ParseCMS(cms_outputs)

	## Calculate the phase portrait
	print("Computing the deterministic phase portrait...")
	I, M, phase = PhasePortrait(params)
	print("finished.")

	if compute_prob_portrait:
		print("Computing the stochastic phase portrait...")
		i, m, p = CMSProbabilityPortrait(params)
		print("finished.")

	fig, axes = plt.subplots(figsize=(8,6))
	for cms_traj in cms_trajs:
		axes = PlotTrajectory(axes,cms_traj,alpha=0.4,markers=False)
	for traj in trajs:
		axes = PlotTrajectory(axes,traj,alpha=1.)
		axes = PlotTrajectory(axes,traj,alpha=1.)
	axes.imshow(phase,extent=(I.min(),I.max(),M.min(),M.max()),vmin=0.,vmax=1.,
			   	   origin="lower",cmap="Greys",aspect="auto",alpha=0.4)
	axes.set(xlabel="Prevalence, I", ylabel="Income, M",title="Deterministic vs stochastic")
	plt.savefig("DvsS_v0.png")


	if compute_prob_portrait:
		fig, axes = plt.subplots(1, 2, sharex=True, sharey=True, figsize=(12,6))
		for cms_traj in cms_trajs:
			axes[0] = PlotTrajectory(axes[0],cms_traj,alpha=0.4,markers=False)
			axes[1] = PlotTrajectory(axes[1],cms_traj,alpha=0.4,markers=False)
		for traj in trajs:
			axes[0] = PlotTrajectory(axes[0],traj,alpha=1.)
			axes[1] = PlotTrajectory(axes[1],traj,alpha=1.)
		axes[0].imshow(phase,extent=(I.min(),I.max(),M.min(),M.max()),vmin=0.,vmax=1.,
				   	   origin="lower",cmap="Greys",aspect="auto",alpha=0.4)
		axes[0].set(xlabel="Prevalence, I", ylabel="Income, M",title="Deterministic")
		im = axes[1].imshow(p, extent=[i.min(), i.max(), m.min(), m.max()], origin="lower",
					cmap="Greys",aspect="auto",alpha=0.4,vmin=0., vmax=1.)
		axes[1].set(xlabel="Prevalence, I", ylabel="Income, M",title="Stochastic")
		plt.tight_layout()
		plt.savefig("DvsS_v1.png")

		## plot the results
		fig, axes = plt.subplots(1, 2, sharex=True, sharey=True, figsize=(12,6))
		for cms_traj in cms_trajs:
			axes[1] = PlotTrajectory(axes[1],cms_traj,alpha=1.,markers=True)
		for traj in trajs:
			axes[0] = PlotTrajectory(axes[0],traj,alpha=1.)
		axes[0].imshow(phase,extent=(I.min(),I.max(),M.min(),M.max()),vmin=0.,vmax=1.,
				   	   origin="lower",cmap="Greys",aspect="auto",alpha=0.4)
		axes[0].set(xlabel="Prevalence, I", ylabel="Income, M",title="Deterministic")
		im = axes[1].imshow(p, extent=[i.min(), i.max(), m.min(), m.max()], origin="lower",
					cmap="Greys",aspect="auto",alpha=0.4,vmin=0., vmax=1.)
		axes[1].set(xlabel="Prevalence, I", ylabel="Income, M",title="Stochastic")
		plt.tight_layout()
		plt.savefig("DvsS_v2.png")

	plt.show()
