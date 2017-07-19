# IDM Compartmental Modeling Software

Basic useful feature list:

 * Multiple solvers
 * Multiple output formats
 * Simple JSON configuration
 * Text based model specification

Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.

Here is a sample configuration: :+1:

```javascript
{
    "duration" : 3650,
    "runs" : 1,
    "samples" : 3650,
    "solver" : "R",
    "output" : {
        "headers" : true
    },
    "tau-leaping" : {
        "epsilon" : 0.01
    },
    "r-leaping" : {}
}
```

Here is a sample model file:

```Scheme
; simplemodel

(import (rnrs) (emodl cmslib))

(start-model "seir.emodl")

(species S 990)
(species E)
(species I 10)
(species R)

(observe susceptible S)
(observe exposed     E)
(observe infectious  I)
(observe recovered   R)

(param Ki 0.0005)
(param Kl 0.2)
(param Kr (/ 1 7))
(param Kw (/ 1 135))

(reaction exposure   (S I) (E I) (* Ki S I))
(reaction infection  (E)   (I)   (* Kl E))
(reaction recovery   (I)   (R)   (* Kr I))
(reaction waning     (R)   (S)   (* Kw R))

(end-model)
```

[GitHub repository](https://github.com/InstituteforDiseaseModeling/CompartmentalModeling)

