==================
Configuration file
==================

The compartmental modeling software does not require a configuration file. However, the default settings for :code:`duration` (100) and :code:`runs` (1) are generally not very useful.

A minimally useful configuration file would specify which solver to use, how many realizations to calculate, the duration of each realization (specified in unit-less time relevant to the timescales of the model), and the number of samples of the various observables to take over the duration of the simulation.

This example specifies the Gillespie Stochastic Simulation Algorithm, 100 realizations, a duration of 730 time units (e.g. two years for a model with rates specified in days), and 250 samples of each realization spaced evenly over the 730 time unit duration.

.. literalinclude:: /json_templates/minimal.json
    :language: JSON

Additional options for the configuration file include control of the output, control of the pseudo-random number generator, and solver specific parameters.

**Output Options**

``prefix``
    Specifies the main name of the output files to be written. The default is "trajectories".

``writecsv``
    Specifies whether or not to write realization data in CSV format. The default is ``true``.

``writerealizationindex``
    Specifies whether or not to add a suffix with the realization index to each observable name in a CSV file. The default is ``true``.

``compress``
    Specifies whether or not CSV and JSON output files should be compressed using gzip or MATLAB .MAT files with internal compression. This can be useful for large data files. The default is ``false``.

.. ``headers``
..     Specifies whether or not CSV files should include a header row with the build version number. The default is ``true``.

``writejson``
    Specifies whether or not to write realization data in JSON format. The default is ``false``.

*Sample JSON output file:*

.. literalinclude:: /json_templates/sample-output.json
    :language: JSON

``channeltitles``
    Specifies whether or not to populate the :code:`ChannelTitles` array in JSON output files. If set to ``true`` the entries of the :code:`ChannelTitles` pair with the channel data in the :code:`ChannelData` array. Each entry in :code:`ChannelTitles` consists of an observable name followed by the realization number in curly braces, e.g. ``"susceptible{0}"``. The default is ``false``.

``writematfile``
    Specifies whether or not to write realization data in MATLAB .mat format. The default is ``false``.

``newmatformat``
    Specifies whether to use the original MATLAB schema or the "new" MATLAB schema.

    The original schema provided for 4 elements in the .MAT file:

    1. version - a string
    2. observables - a # of observables rows x 1 column cell matrix with the names of the observable quantities
    3. sampletimes - a 1 row x # of samples column matrix of sample times
    4. data - a (# of observables x # of realizations) rows x # of samples columns matrix

    The "new" schema is as follows:

    1. version - a string
    2. sampletimes - a 1 row x # of samples column matrix of sample times
    3. observable1...observableN - each entry a # of realizations row x # of samples column matrix

    The default is ``false``.

*Sample output configuration (defaults):*

.. literalinclude:: /json_templates/output-config.json
    :language: JSON

**Pseudo-Random Number Generator**

See the :doc:`random number library <random-number-library>` page for pseudo-random number configuration parameters.

**Solver Specific Configuration**

See the individual page for each :doc:`solver <solvers>` for solver specific configuration parameters.
