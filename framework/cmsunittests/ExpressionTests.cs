using System;
using NUnit.Framework;

using compartments.emod.interfaces;
using compartments.emod.expressions;

namespace cmsunittests
{
    [TestFixture, Description("Expression support objects tests")]
    class ExpressionTests : AssertionHelper
    {
        class BooleanTrue : IBoolean
        {
            public bool Value
            {
                get { return true; }
            }
        }

        class BooleanFalse : IBoolean
        {
            public bool Value
            {
                get { return false; }
            }
        }

        [Test, Description("Boolean And expression tests")]
        public void TestBooleanAnd()
        {
            IBoolean andTrueTrue = (new And(new BooleanTrue(), new BooleanTrue()));
            IBoolean andTrueFalse  = (new And(new BooleanTrue(), new BooleanFalse()));
            IBoolean andFalseTrue  = (new And(new BooleanFalse(), new BooleanTrue()));
            IBoolean andFalseFalse = (new And(new BooleanFalse(), new BooleanFalse()));

            Console.WriteLine("Testing And() expressions...");
            Console.WriteLine();

            Expect(andTrueTrue.Value);
            Expect(!andTrueFalse.Value);
            Expect(!andFalseTrue.Value);
            Expect(!andFalseFalse.Value);
        }

        [Test, Description("Boolean Or expression tests")]
        public void TestBooleanOr()
        {
            IBoolean orTrueTrue   = new Or(new BooleanTrue(), new BooleanTrue());
            IBoolean orTrueFalse  = new Or(new BooleanTrue(), new BooleanFalse());
            IBoolean orFalseTrue  = new Or(new BooleanFalse(), new BooleanTrue());
            IBoolean orFalseFalse = new Or(new BooleanFalse(), new BooleanFalse());

            Console.WriteLine("Testing Or() expressions...");
            Console.WriteLine();

            Expect(orTrueTrue.Value);
            Expect(orTrueFalse.Value);
            Expect(orFalseTrue.Value);
            Expect(!orFalseFalse.Value);
        }

        [Test, Description("Boolean Xor expression tests")]
        public void TestBooleanXor()
        {
            IBoolean xorTrueTrue   = new Xor(new BooleanTrue(), new BooleanTrue());
            IBoolean xorTrueFalse  = new Xor(new BooleanTrue(), new BooleanFalse());
            IBoolean xorFalseTrue  = new Xor(new BooleanFalse(), new BooleanTrue());
            IBoolean xorFalseFalse = new Xor(new BooleanFalse(), new BooleanFalse());

            Console.WriteLine("Testing Xor() expressions...");
            Console.WriteLine();

            Expect(!xorTrueTrue.Value);
            Expect(xorTrueFalse.Value);
            Expect(xorFalseTrue.Value);
            Expect(!xorFalseFalse.Value);
        }

//        private readonly INumericOperator _oneHalf       = new Constant(0.5f);
        private readonly INumericOperator _two           = new Constant(2.0f);
        private readonly INumericOperator _twenty        = new Constant(20.0f);
        private readonly INumericOperator _twentyTwo     = new Constant(22.0f);
        private readonly INumericOperator _negativeThree = new Constant(-3.0f);
        private readonly INumericOperator _pi            = new Constant(3.14159265f);
        private readonly INumericOperator _e             = new Constant(2.718281828459f);

        [Test, Description("AddOperator test")]
        public void AdderTest()
        {
            IValue adder = (new AddOperator(_twenty, _twentyTwo)).ResolveReferences(null);

            Console.WriteLine("Testing AddOperator...");
            Console.WriteLine();

            Expect(adder.Value == 42.0f);
        }

        [Test, Description("SubtractOperator test")]
        public void SubtractTest()
        {
            IValue subtract = (new SubtractOperator(_twenty, _twentyTwo)).ResolveReferences(null);

            Console.WriteLine("Testing SubtractOperator...");
            Console.WriteLine();

            Expect(subtract.Value == -2.0f);
        }

        [Test, Description("MultiplyOperator test")]
        public void MultiplyTest()
        {
            IValue multiply = (new MultiplyOperator(_twenty, _twentyTwo)).ResolveReferences(null);

            Console.WriteLine("Testing MultiplyOperator...");
            Console.WriteLine();

            Expect(multiply.Value == 440.0f);
        }

        [Test, Description("DivideOperator test")]
        public void DivideTest()
        {
            IValue divide = (new DivideOperator(_twentyTwo, _twenty)).ResolveReferences(null);

            Console.WriteLine("Testing DivideOperator...");
            Console.WriteLine();

            Expect(divide.Value == 1.1f);
        }

        [Test, Description("ModuloOperator test 22 % 20 == 2")]
        public void ModuloTest1()
        {
            IValue modulo = (new ModuloOperator(_twentyTwo, _twenty)).ResolveReferences(null);

            Console.WriteLine("Testing ModuloOperator...");
            Console.WriteLine();

            Expect(modulo.Value == 2.0f);
        }

        [Test, Description("ModuloOperator test 22 % pi == 1")]
        public void ModuloTest2()
        {
            IValue modulo = (new ModuloOperator(_twentyTwo, _pi)).ResolveReferences(null);

            Console.WriteLine("Testing ModuloOperator...");
            Console.WriteLine();

            Expect(modulo.Value == 1.0f);
        }

        [Test, Description("ModuloOperator test e % 22 == 2")]
        public void ModuloTest3()
        {
            IValue modulo = (new ModuloOperator(_e, _twentyTwo)).ResolveReferences(null);

            Console.WriteLine("Testing ModuloOperator...");
            Console.WriteLine();

            Expect(modulo.Value == 2.0f);
        }

        [Test, Description("ModuloOperator test pi % e == 1")]
        public void ModuloTest4()
        {
            IValue modulo = (new ModuloOperator(_pi, _e)).ResolveReferences(null);

            Console.WriteLine("Testing ModuloOperator...");
            Console.WriteLine();

            Expect(modulo.Value == 1.0f);
        }

        [Test, Description("PowerOperator test")]
        public void PowerTest()
        {
            IValue power = (new PowerOperator(_twenty, _twentyTwo)).ResolveReferences(null);

            Console.WriteLine("Testing PowerOperator...");
            Console.WriteLine();

            Expect(power.Value == 4.1943039069687888944681713664E28f);
        }

        [Test, Description("MaximumOperator test")]
        public void MaximumTest()
        {
            IValue maximum1 = (new MaximumOperator(_twenty, _twentyTwo)).ResolveReferences(null);
            IValue maximum2 = (new MaximumOperator(_twentyTwo, _twenty)).ResolveReferences(null);

            Console.WriteLine("Testing MaximumOperator...");
            Console.WriteLine();

            Expect(maximum1.Value == 22.0f);
            Expect(maximum2.Value == 22.0f);
        }

        [Test, Description("MinimumOperator test")]
        public void MinimumTest()
        {
            IValue minimum1 = (new MinimumOperator(_twenty, _twentyTwo)).ResolveReferences(null);
            IValue minimum2 = (new MinimumOperator(_twentyTwo, _twenty)).ResolveReferences(null);

            Console.WriteLine("Testing MinimumOperator...");
            Console.WriteLine();

            Expect(minimum1.Value == 20.0f);
            Expect(minimum2.Value == 20.0f);
        }

        [Test, Description("LessThan test")]
        public void LessThanTest()
        {
            IBoolean lessThanTrue  = (new LessThanOperator(_twenty, _twentyTwo)).ResolveReferences(null, null);
            IBoolean lessThanFalse = (new LessThanOperator(_twentyTwo, _twenty)).ResolveReferences(null, null);

            Console.WriteLine("Testing LessThan() expressions...");
            Console.WriteLine();

            Expect(lessThanTrue.Value);
            Expect(!lessThanFalse.Value);
        }

        [Test, Description("LessThanOrEqual test")]
        public void LessThanOrEqualTest()
        {
            IBoolean lessThanOrEqual1 = (new LessThanOrEqualOperator(_twenty, _twenty)).ResolveReferences(null, null);
            IBoolean lessThanOrEqual2 = (new LessThanOrEqualOperator(_twenty, _twentyTwo)).ResolveReferences(null, null);
            IBoolean lessThanOrEqual3 = (new LessThanOrEqualOperator(_twentyTwo, _twenty)).ResolveReferences(null, null);

            Console.WriteLine("Testing LessThanOrEqual() expressions...");
            Console.WriteLine();

            Expect(lessThanOrEqual1.Value);
            Expect(lessThanOrEqual2.Value);
            Expect(!lessThanOrEqual3.Value);
        }

        [Test, Description("GreaterThan test")]
        public void GreaterThanTest()
        {
            IBoolean greaterThanTrue  = (new GreaterThanOperator(_twentyTwo, _twenty)).ResolveReferences(null, null);
            IBoolean greaterThanFalse = (new GreaterThanOperator(_twenty, _twentyTwo)).ResolveReferences(null, null);

            Console.WriteLine("Testing GreaterThan() expressions...");
            Console.WriteLine();

            Expect(greaterThanTrue.Value);
            Expect(!greaterThanFalse.Value);
        }

        [Test, Description("GreaterThanOrEqual test")]
        public void GreaterThanOrEqualTest()
        {
            IBoolean greaterThanOrEqual1 = (new GreaterThanOrEqualOperator(_twenty, _twenty)).ResolveReferences(null, null);
            IBoolean greaterThanOrEqual2 = (new GreaterThanOrEqualOperator(_twenty, _twentyTwo)).ResolveReferences(null, null);
            IBoolean greaterThanOrEqual3 = (new GreaterThanOrEqualOperator(_twentyTwo, _twenty)).ResolveReferences(null, null);

            Console.WriteLine("Testing GreaterThanOrEqual() expressions...");
            Console.WriteLine();

            Expect(greaterThanOrEqual1.Value);
            Expect(!greaterThanOrEqual2.Value);
            Expect(greaterThanOrEqual3.Value);
        }

        [Test, Description("EqualTo test")]
        public void EqualToTest()
        {
            IBoolean equalTo1 = (new EqualToOperator(_twenty, _twenty)).ResolveReferences(null, null);
            IBoolean equalTo2 = (new EqualToOperator(_twenty, _twentyTwo)).ResolveReferences(null, null);
            IBoolean equalTo3 = (new EqualToOperator(_twentyTwo, _twenty)).ResolveReferences(null, null);

            Console.WriteLine("Testing EqualTo() expressions...");
            Console.WriteLine();

            Expect(equalTo1.Value);
            Expect(!equalTo2.Value);
            Expect(!equalTo3.Value);
        }

        [Test, Description("NotEqualTo test")]
        public void NotEqualToTest()
        {
            IBoolean notEqualTo1 = (new NotEqualToOperator(_twenty, _twenty)).ResolveReferences(null, null);
            IBoolean notEqualTo2 = (new NotEqualToOperator(_twenty, _twentyTwo)).ResolveReferences(null, null);
            IBoolean notEqualTo3 = (new NotEqualToOperator(_twentyTwo, _twenty)).ResolveReferences(null, null);

            Console.WriteLine("Testing NotEqualTo() expressions...");
            Console.WriteLine();

            Expect(!notEqualTo1.Value);
            Expect(notEqualTo2.Value);
            Expect(notEqualTo3.Value);
        }

        [Test, Description("Constant test")]
        public void ConstantTest()
        {
            IValue constant = (new Constant(42.0f)).ResolveReferences(null);

            Console.WriteLine("Testing Constant() expressions...");
            Console.WriteLine();

            Expect(constant.Value == 42.0f);
        }

        [Test, Description("Symbol test")]
        [Ignore("No symbol resolution yet.")]
        public void SymbolTest()
        {
/*
            IValue symbol = new SymbolReference("test");

            Console.WriteLine("Testing Symbol() expressions...");
            Console.WriteLine();

            Expect(false);
*/
        }

        [Test, Description("Boolean Not tests")]
        public void NotTest()
        {
            IBoolean notFalse = new Not(new BooleanFalse());
            IBoolean notTrue = new Not(new BooleanTrue());

            Console.WriteLine("Testing Not() expressions...");
            Console.WriteLine();

            Expect(notFalse.Value);
            Expect(!notTrue.Value);
        }

        [Test, Description("ExponentiationOperator test")]
        public void ExponentiateTest()
        {
            IValue exponentiate = (new ExponentiationOperator(_two)).ResolveReferences(null);

            Expect(exponentiate.Value == 7.38905620574951171875f);
        }

        [Test, Description("LogarithmOperator test")]
        public void LogarithmTest()
        {
            IValue logarithm = (new LogarithmOperator(_two)).ResolveReferences(null);

            Expect(logarithm.Value == 6.93147182464599609375E-1f);
        }

        [Test, Description("SineOperator test")]
        public void SineTest()
        {
            IValue sine = (new SineOperator(_two)).ResolveReferences(null);

            Expect(sine.Value == 0.909297406673431396484375f);
        }

        [Test, Description("CosineOperator test")]
        public void CosineTest()
        {
            IValue cosine = (new CosineOperator(_two)).ResolveReferences(null);

            Expect(cosine.Value == -0.4161468446254730224609375f);
        }

        [Test, Description("TangentOperator test")]
        [Ignore("Tangent not currently supported.")]
        public void TangentTest()
        {
/*
            IValue tangent = new TangentOperator(_two);

            Expect(tangent.Value == -2.1850397586822509765625f);
*/
        }

        [Test, Description("ArcSineOperator test")]
        [Ignore("ArcSine not currently supported.")]
        public void ArcSineTest()
        {
/*
            IValue arcSine = new ArcSineOperator(_oneHalf);

            Expect(arcSine.Value == 0.52359879016876220703125f);
*/
        }

        [Test, Description("ArcCosineOperator test")]
        [Ignore("ArcCosine not currently supported.")]
        public void ArcCosineTest()
        {
/*
            IValue arcCosine = new ArcCosineOperator(_oneHalf);

            Expect(arcCosine.Value == 1.0471975803375244140625f);
*/
        }

        [Test, Description("ArcTangentOperator test")]
        [Ignore("ArcTangent not currently supported.")]
        public void ArcTangentTest()
        {
/*
            IValue arcTangent = new ArcTangentOperator(_two);

            Expect(arcTangent.Value == 1.10714876651763916015625f);
*/
        }

        [Test, Description("AbsoluteOperator value test")]
        public void AbsTest()
        {
            IValue absValuePositive = (new AbsoluteOperator(_two)).ResolveReferences(null);
            IValue absValueNegative = (new AbsoluteOperator(_negativeThree)).ResolveReferences(null);

            Expect(absValuePositive.Value == 2.0f);
            Expect(absValueNegative.Value == 3.0f);
        }

        [Test, Description("FloorOperator test")]
        public void FloorTest()
        {
            IValue floor1 = (new FloorOperator(new Constant(2.75f))).ResolveReferences(null);
            IValue floor2 = (new FloorOperator(new Constant(2.25f))).ResolveReferences(null);
            IValue floor3 = (new FloorOperator(new Constant(-2.25f))).ResolveReferences(null);
            IValue floor4 = (new FloorOperator(new Constant(-2.75f))).ResolveReferences(null);

            Expect(floor1.Value == 2.0f);
            Expect(floor2.Value == 2.0f);
            Expect(floor3.Value == -3.0f);
            Expect(floor4.Value == -3.0f);
        }

        [Test, Description("CeilingOperator test")]
        public void CeilingTest()
        {
            IValue ceiling1 = (new CeilingOperator(new Constant(2.75f))).ResolveReferences(null);
            IValue ceiling2 = (new CeilingOperator(new Constant(2.25f))).ResolveReferences(null);
            IValue ceiling3 = (new CeilingOperator(new Constant(-2.25f))).ResolveReferences(null);
            IValue ceiling4 = (new CeilingOperator(new Constant(-2.75f))).ResolveReferences(null);

            Expect(ceiling1.Value == 3.0f);
            Expect(ceiling2.Value == 3.0f);
            Expect(ceiling3.Value == -2.0f);
            Expect(ceiling4.Value == -2.0f);
        }

        [Test, Description("Square root test")]
        public void SquareRootTest()
        {
            IValue squareRoot = (new SqrtOperator(_two)).ResolveReferences(null);

            Expect(squareRoot.Value == 1.41421353816986083984375f);
        }

        [Test, Description("NegateOperator test")]
        public void NegateTest()
        {
            IValue negatePositive = (new NegateOperator(_two)).ResolveReferences(null);
            IValue negateNegative = (new NegateOperator(new Constant(-3.0f))).ResolveReferences(null);

            Expect(negatePositive.Value == -2.0f);
            Expect(negateNegative.Value == 3.0f);
        }

        [Test, Description("HeavisideStepOperator test")]
        public void HeavisideStepTest()
        {
            IValue stepPositive = (new HeavisideStepOperator(_two)).ResolveReferences(null);
            IValue stepZero     = (new HeavisideStepOperator(new Constant(0.0f))).ResolveReferences(null);
            IValue stepNegative = (new HeavisideStepOperator(new Constant(-3.0f))).ResolveReferences(null);

            Expect(stepPositive.Value == 1.0f);
            Expect(stepZero.Value == 1.0f);
            Expect(stepNegative.Value == 0.0f);
        }
    }
}
