======================
Create a spatial model
======================

Spatial structure is an important component in many disease modeling. CMS offers an easy way to specify geographic compartments. The syntax for creating a locale is ``(locale name)``, and all  :doc:`spatial solvers <Spatial Simulation Methods>` support this feature. 

Species must be specified with the name of the local where it belongs in the following for mat: ``species_name::locale_name``. The below example illustrates how locales and species are created for a spatial simulation. 


.. literalinclude:: /emodl_templates/TransportSSA.emodl
	:language: lisp

	