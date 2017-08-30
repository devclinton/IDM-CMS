=================
CMS installation
=================



#. Download ``compartments.zip`` from the CompartmentalModeling repository on GitHub_.
#. Unzip ``compartments.zip`` to an appropriate location.
#. Reference ``compartments.exe`` from the zip file with a full path, e.g., ``c:\cms\compartments.exe`` *or*
#. Add the path to ``compartments.exe`` to the ``PATH`` environment variable, e.g. ``set PATH=%PATH%;c:\cms``\*

\*Note that using the ``set`` command will only affect the current command line window. Use ``setx PATH "%PATH%;c:\cms"`` to permanently modify the ``PATH`` environment variable. Note that the change to ``PATH`` will not be visible in the current command window so you will need to also use the basic ``set`` command above or open a new command window.

.. _GitHub: https://github.com/InstituteforDiseaseModeling/CompartmentalModeling/releases