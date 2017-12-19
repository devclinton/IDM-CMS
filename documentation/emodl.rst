==============================================
EMODL – model language for an input model file
==============================================

Boiler plate:

``(import (rnrs) (emodl cmslib))``
``(start-model "modelname")``
...
``(end-model)``

Basic ``EMODL`` syntax:

:bool:        ``(bool name expression)`` defines a function which returns a boolean value, used with ``state-event``
:func:        ``(func name function)`` defines a numeric function which is evaluated each time it is needed
:locale:      ``(locale name)`` creates a new geographic locale
:observe:     ``(observe label function)`` adds a variable or function to the list of observed items which are output from a simulation
:param:       ``(param name value)`` defines a named, constant value
:reaction:    ``(reaction name input-species output-species propensity-function)`` defines a reaction or transition from one set of species to another
:set-locale:  ``(set-local name)`` sets the current geographic locale, new species will be associated with this geopgraphic locale, used with spatial solvers
:species:     ``(species name identifier [initial population])`` defines a unique species or population of particles or agents

:time-event:  ``(time-event name time iterations (variable-value pairs))`` defines an event to occur at a particular time, optionally with recurrence
:state-event: ``(state-event name predicate (variable-value pairs))`` defines an event to occur given a particular system state
:json:        declares an external JSON formatted file which can be referenced in species definitions, param definitions, and functions

``(variable-value pairs)`` is a list, (...), of pairs, (var val), of variables to set and the value to which to set them, e.g. ((V (* S 0.5)) (S (* S 0.5))) sets V = S/2 and then sets S = S/2 (i.e. transfer half the population of S to V).

.. _function_list:

Mathematical operators and functions:

*Unary*

:negate:         ``(- x)``
:exponentiation: ``(exp x)`` returns e^x
:logarithm:      ``(ln x)``
:sine:           ``(sin x)``
:cosine:         ``(cos x)``
:absolute:       ``(abs x)``
:floor:          ``(floor x)`` returns the largest integer <= x
:ceiling:        ``(ceil x)`` returns the smallest integer >= x
:sqrt:           ``(sqrt x)``
:Heaviside step: ``(step x)`` returns 1 if x >= 0 else returns 0
:empirical:      ``(empirical "filename")`` reads an empirically defined CDF from the file specified and returns a probability from the CFG based on a random number draw

*Binary*

:add:            ``(+ x y)``
:subtract:       ``(- x y)``
:multiply:       ``(* x y)``
:divide:         ``(/ x y)``
:power:          ``(^ x y)`` or ``(pow x y)``
:minimum:        ``(min x y)``
:maximum:        ``(max x y)``
:uniform:        ``(uniform min max)`` returns a value uniformly distributed between min and max based on a random number draw
:normal:         ``(normal mean var)`` or ``(gaussian mean var)`` returns a value from a normal distribution with the given mean and variance

*N-ary*

:add:            ``(+ x y z ...)`` or ``(sum x y z ...)`` returns the sum of all the given arguments
:multiply:       ``(* x y z ...)`` returns the product of all the given arguments

