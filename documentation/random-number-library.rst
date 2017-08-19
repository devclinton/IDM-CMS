=====================
Random number library
=====================

CMS supports four different pseudo-random number generators. These can be specified with "RNG" : { "type": *prng-name* } in the configuration file. :code:`AESCOUNTER` is the default prng unless not supported in hardware in which case the compartmental modeling software falls back to :code:`PSEUDODES`.

The configuration file may optionally include the :code:`prng_seed` and :code:`prng_index` keys. These values are used to initialize the pseudo-random number generator. They both default to 0. :code:`prng_seed` and :code:`prng_index` together determine the initial state of the pseudo-random number generator. :code:`prng_index` can be used to identify different runs of an experiment or to seed different instantiations of the compartmental modeling software across multiple processors.

.. literalinclude:: /json_templates/prng.json
    :language: JSON

**VANILLA**

The :code:`VANILLA` prng is based on the .NET Random_ class.

.. _Random: https://msdn.microsoft.com/en-us/library/system.random(v=vs.110).aspx

**RANDLIB**

The :code:`RANDLIB` prng is based on the pseudo-random number generator based on a combination of multiplicative linear congruential generators proposed by Pierre L'Ecuyer [1]_ and used in numerous scientific computing libraries.

**PSEUDODES**

The :code:`PSEUDODES` prng is based on the entropy generating step of the Data Encryption Standard published by NIST. The algorithm is ``psdes`` described in "`Numerical Recipes in C <http://numerical.recipes/>`_."

**AESCOUNTER**

The :code:`AESCOUNTER` prng is similar in concept to :code:`PSEUDODES` and is based on the entropy generating step of the `Advanced Encryption Standard <http://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.197.pdf>`_ published by NIST in 2001. The implementation uses `AES Counter Mode <http://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-38a.pdf>`_ to generate a stream a pseudo-random numbers from the given seed values.

.. [1] `Efficient and Portable Combined Random Number Generators, Communications of the ACM, June 1988 <https://www.iro.umontreal.ca/~lecuyer/myftp/papers/cacm88.pdf>`_
