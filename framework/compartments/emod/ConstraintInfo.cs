/***************************************************************************************************

Copyright (c) 2018 Intellectual Ventures Property Holdings, LLC (IVPH) All rights reserved.

EMOD is licensed under the Creative Commons Attribution-Noncommercial-ShareAlike 4.0 License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode

***************************************************************************************************/

using System;

namespace compartments.emod
{
    /*
     * TODO - implement the predicate for Constraint and connect it to the species, expressions, etc. on which it depends.
     * TODO - implement a callback/delegate mechanism so a model's constraints don't have to be evaluated manually on each time step.
     */

    public class ConstraintInfo
    {
        public ConstraintInfo(BooleanExpressionTree predicate)
        {
            Name      = String.Empty;
            Predicate = predicate;
        }

        public ConstraintInfo(String name, BooleanExpressionTree predicate)
        {
            Name      = name;
            Predicate = predicate;
        }

        public string Name { get; private set; }

        public BooleanExpressionTree Predicate { get; private set; }

        public override string ToString()
        {
            return string.Format("(check {0}{1})", (Name != null ? string.Format("{0} ", Name) : String.Empty), Predicate);
        }
    }
}
