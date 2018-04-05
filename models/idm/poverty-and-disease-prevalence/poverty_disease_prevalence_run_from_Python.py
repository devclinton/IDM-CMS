""" Python functions to interact with CMS, run trajectories and compare with the 
deterministic model."""
from __future__ import print_function

## Standard stuff
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

## For running the cms.exe
import os

## Custom global matplotlib parameters
## see http://matplotlib.org/users/customizing.html for details.
plt.rcParams["font.size"] = 14.
plt.rcParams["font.family"] = "serif"
plt.rcParams["font.sans-serif"] = "DejaVu Sans"
plt.rcParams["font.serif"] = "Times New Roman"
plt.rcParams["xtick.labelsize"] = "small"
plt.rcParams["ytick.labelsize"] = "small"
plt.rcParams["legend.fontsize"] = "small"
plt.rcParams["axes.linewidth"] = 1.0

## Functions to create emodl and cfg files.
######################################################################################
def EMODL(ics,params,fname="poverty_disease_prevalence.emodl",save=True):

	""" Function to modify the poverty_trap.emodl """

	base = """; This file was auto-generated via python
	(import (rnrs) (emodl cmslib))
	(start-model "poverty_disease_prevalence.emodl")

	; Global model parameters
	(param alpha 0.0) ; birth rate
	(param mu 0.0) ; susceptible death rate
	(param nu 0.0) ; increase in death rate for infecteds
	(param r 0.0) ; income's rate parameter
	(param beta1 0.0) ; maximum transmission rate
	(param gamma1 0.0) ; maximum recovery rate
	(param epsilon 0.0) ; beta(income) = beta1*epsilon/(income + epsilon)
	(param kappa 0.0) ; gamma(income) = gamma1*income/(income + kappa)
	(param M0 0.0) ; max income

	; Then set the initial populations and
	; initial income.
	(species S 0)
	(species I 0)
	(species M 0)

	; Compute the total population
	(func totalpop (+ S I))

	; Given M, you can compute the M dependent rates
	(func gamma (/ (* gamma1 M) (+ M kappa)))  ; recovery rate
	(func beta (/ (* beta1 epsilon) (+ M epsilon))) ; transmission rate


	; What gets collected and output at 
	; every timestep
	(observe susceptible S)
	(observe infected I)
	(observe income M)
	(observe population totalpop)

	; Set the reactions
	(reaction birth             ()   (S)  (* alpha totalpop))
	(reaction deathS            (S)  ()   (* S mu))
	(reaction deathI            (I)  ()   (* I (+ mu nu)))
	(reaction infection         (S)  (I)  (* (/ I totalpop) beta S))
	(reaction recovery          (I)  (S)  (* gamma I))
	(reaction m1                (M)  ()   (* r M M))
	(reaction m2                ()   (M)  (* r M0 M (/ S totalpop)))

	; End the model
	(end-model)"""
	base = base.replace("	","")

	## Assumed parameter names in the same order as
	## params
	param_names = ["alpha", "mu","nu", "r", "beta1", "gamma1", "epsilon", "kappa", "M0"]

	## Change the model name
	base = base.replace('(start-model "poverty_disease_prevalence.emodl")', '(start-model "'+fname+'")')

	## Modify the parameter values
	for name, param in zip(param_names, params):
		s1 = "(param "+name+" 0.0)"
		s2 = "(param "+name+" "+str(param)+")"
		base = base.replace(s1,s2)
	
	## And modify the IC's
	assert len(ics) == 3, "Incorrect IC shape for the CMS model. Needs to be [S,I,M]"
	for name, ic in zip(["S","I","M"],ics):
		s1 = "(species "+name+" 0)"
		s2 = "(species "+name+" "+str(int(ic))+")"
		base = base.replace(s1,s2)

	## Save to file
	if save:
		file = open(fname,"w")
		file.write(base)
		file.close()

	return base

def CFG(num_runs=1,fname="poverty_disease_prevalence_config.cfg",save=True):

	"""Write the config file."""

	base ="""
	{
    	"duration" : 150,
    	"runs" : 0,
    	"samples" : 150,
    	"solver" : "SSA",
    	"output" : {
        	"headers" : true
    	}
	}"""
	base = base.replace("	","")

	## Adjust the number of runs
	base = base.replace('"runs" : 0,', '"runs" : '+str(num_runs)+',')

	## save if required
	if save:
		file = open(fname,"w")
		file.write(base)
		file.close()

	return base

## Run CMS via os.system()
######################################################################################
def RunCMS(model="poverty_disease_prevalence.emodl",config="poverty_disease_prevalence_config.cfg",verbose=True):
	
	## This assumed that you're in a directory
	## within the CMS folder, so the .exe is one directory
	## above.
	command = "..\\compartments.exe -model "+model+" -config "+config
	if verbose:
		os.system(command)
	else:
		os.system(command+" > null")
		os.system("rm null")


## plotting and interacting with trajectories.csv
######################################################################################
def GetTrajectories(fname="trajectories.csv"):

	## Get the trajectories
	df = pd.read_csv(fname,skiprows=1,index_col=0)

	## Add a trajectory number
	traj_number = [int(s[s.find("{")+1:s.find("}")]) for s in df.index]
	df["trajectory"] = traj_number

	## Clean up a little
	del df.index.name

	return df

def PovertyTrapTrajectory(df, alpha=0.1, legend=True):

	""" Plot trajectories divided by the total population. DF must have a key
	'population' """

	fig, axes = plt.subplots(figsize=(12,6))
	colors = ["C0","C2","C2"]

	## Set the labels if lengend=True
	if legend:
		#compartments = ["Susceptible", "Infected", "Income"]
		compartments = ["Infected","Income"]
		for i,c in enumerate(compartments):
			color = colors[i]
			axes.plot([],label=c,color=color)
		axes.legend(loc=2)

	## Plot trajectory by trajectory
	for i,sf in df.groupby("trajectory"):

		## Since everything is the same trajectory now
		sf = sf.drop("trajectory",axis=1)

		## Normalize the appropriate ones by pop
		s = "{"+str(i)+"}"
		sf.loc["S"] = sf.loc["susceptible"+s] / sf.loc["population"+s]
		sf.loc["I"] = sf.loc["infected"+s] / sf.loc["population"+s]
		sf.loc["M"] = sf.loc["income"+s] / 100.

		## Plot the trajectories for each bin
		for j,s in enumerate(["I","M"]):
			color = colors[j]
			axes.plot(sf.loc[s],color=color,alpha=alpha,label="_nolegend_")
	
	## Set the axis labels
	axes.set(title="Simulation trajectories",xlabel="Time",ylabel="Population")
	return fig, axes

def SamplePovertyTrapPlot(df, num_samples=50, alpha=0.1, legend=True):

	"""Make a plot like above but with a random sample of the trajectories (good if
	you have a ton a traces)."""

	## Resample the frame
	trajectories = np.random.choice(df["trajectory"].value_counts().index.tolist(),
								    size=(num_samples,), replace=False)
	sf = df[df["trajectory"].isin(trajectories)]

	## Make the plot
	return PovertyTrapTrajectory(sf,alpha=alpha,legend=legend)

if __name__ == "__main__":

	## Parameters (needs to be the same as poverty_disease_prevalence.emodl)
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

	ic = np.array([50, 75, 45])
	emodl = EMODL(ic,params)
	cfg = CFG(100)
	RunCMS()

	## Plot it up
	df = GetTrajectories()
	fig, axes = SamplePovertyTrapPlot(df)
	plt.show()




