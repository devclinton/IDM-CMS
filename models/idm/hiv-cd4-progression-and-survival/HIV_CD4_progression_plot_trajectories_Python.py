# Daniel J. Klein, IDM.  Sept 12th, 2017.
import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt
import re

traj = pd.read_csv('trajectories.csv', skipinitialspace=True, skiprows=1, header=None) \
    .set_index(0) \
    .transpose() \
    .set_index('sampletimes')


traj.columns.name = 'Name'
traj = traj.stack() \
    .to_frame() \
    .reset_index('Name')

traj.rename(columns={0:'Value'}, inplace=True)

def extract_sample(col):
    #print [re.match('/{(.*?)}/', c) for c in col.values]
    return [int(float(re.findall(r'\{\d+\}', c)[0][1:-1])) for c in col.values]

def extract_variable(col):
    return [c.split('{')[0] for c in col.values]

traj['Sample'] = extract_sample(traj['Name'])
traj['Variable'] = extract_variable(traj['Name'])
traj.reset_index(inplace=True)

print 'Plotting ...'
ax = sns.tsplot(data=traj, time='sampletimes', unit='Sample', condition='Variable', value='Value')

plt.savefig('Trajectories.png')
