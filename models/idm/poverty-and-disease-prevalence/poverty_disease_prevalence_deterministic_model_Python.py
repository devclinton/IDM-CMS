""" Reproduce some deterministic results from the Pulcinski et al paper. """
from __future__ import print_function

import numpy as np
import matplotlib.pyplot as plt
np.random.seed(123)

## ODE integrator
from scipy.integrate import odeint

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

def dxdt(x,t,*args):

	## unpack the state
	I, M = x

	## unpack the params:
	alpha, nu, r, beta1, gamma1, epsilon, kappa, M0 = args

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

def PlotTrajectory(axes, traj, alpha=0.6):

	if traj[-1,0] >= 0.99:
		color = "C4"
	else:
		color = "C0"

	axes.plot(traj[:,0],traj[:,1],c=color,alpha=alpha)
	axes.plot([traj[0,0], traj[-1,0]], [traj[0,1], traj[-1,1]], 
			  c="k",marker="o",ls="None",alpha=alpha)
	return axes


if __name__ == "__main__":


	## Parameters
	alpha = 0.06
	nu = 0.02
	r = 0.05
	beta1 = 40.
	gamma1 = 0.15
	epsilon = 0.3
	kappa = 100.
	M0 = 100.
	params = np.array([alpha,nu,r,beta1,gamma1,epsilon,kappa,M0])

	## time
	t = np.linspace(0., 1000., 10000)

	## Figure 2a
	## I equation first
	M = np.linspace(0., M0, 100)
	beta = beta1*epsilon/(M+epsilon)
	gamma = gamma1*M/(M+kappa)
	Istar = np.maximum(np.zeros(M.shape), (beta - gamma - alpha - nu)/(beta-nu))

	## Mstar next
	I = np.linspace(0., 1., 100)
	Mstar = M0*(1.-I)

	## Figures
	fig, axes = plt.subplots(1,2,sharex=True,figsize=(12,6))
	axes[0].plot(M,beta,c="k",lw=2.)
	axes[1].plot(M,gamma,c="k",lw=2)
	axes[0].set(xlabel="M",ylabel="beta")
	axes[1].set(xlabel="M",ylabel="gamma")

	## Figure 2
	## Panel 1
	fig, axes = plt.subplots(1,2,sharex=True,sharey=True,figsize=(12,6))
	ics = np.random.uniform(low=[0., 0.], high=[1., M0], size=(10,2))
	trajs = [Trajectory(ic,t,params) for ic in ics]
	for traj in trajs:
		axes[0] = PlotTrajectory(axes[0],traj,alpha=1.)
	axes[0].plot(Istar,M,c="k",ls="dashed",lw=2)
	axes[0].plot(I,Mstar,c="C3",ls="dashed",lw=2)
	axes[0].set(xlabel="Prevalence, I", ylabel="Income, M",title="Nullclines")

	## Panel 2
	t = np.linspace(0.,10.,100)
	M = np.linspace(0.,M0,100)
	I = np.linspace(0.,1.,100)
	ics = np.array([[i,m] for m in M for i in I])
	phase = np.zeros((ics.shape[0],))
	for i, ic in enumerate(ics):
		traj = Trajectory(ic,t,params)
		if traj[-1,0] < 0.5:
			phase[i] = 1.
		else:
			phase[i] = 0.
	phase = np.reshape(phase, (len(I),len(M)))
	axes[1].imshow(phase,extent=(I.min(),I.max(),M.min(),M.max()),vmin=0.,vmax=1.,
			   origin="lower",cmap="Greys",aspect="auto",alpha=0.4)
	for traj in trajs:
		axes[1] = PlotTrajectory(axes[1],traj,alpha=1.)
	axes[1].set(xlabel="Prevalence, I", ylabel="Income, M",title="Phase portrait")
	plt.tight_layout()
	plt.savefig("deterministic.pdf")
	plt.show()
