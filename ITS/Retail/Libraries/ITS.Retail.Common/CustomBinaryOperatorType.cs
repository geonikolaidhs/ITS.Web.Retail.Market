namespace ITS.Retail.Common
{
    //
    // Summary:
    //     Enumerates binary operator types.
    public enum CustomBinaryOperatorType
    {
        //
        // Summary:
        //     Represents the Boolean equality operator.
        //     To create the Boolean equality operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 = Field2")
        Equal = 0,
        //
        // Summary:
        //     Represents the Boolean inequality operator.
        //     To create the Boolean inequality operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 != Field2") or CriteriaOperator.Parse("Field1
        //     Field2")
        NotEqual = 1,
        //
        // Summary:
        //     Represents the Boolean greater-than operator.
        //     To create the Boolean greater-than operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 > Field2")
        Greater = 2,
        //
        // Summary:
        //     Represents the Boolean less-than operator.
        //     To create the Boolean less-than operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 < Field2")
        Less = 3,
        //
        // Summary:
        //     Represents the Boolean less-than-or-equal-to operator.
        //     To create the Boolean less-than-or-equal-to operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 <= Field2")
        LessOrEqual = 4,
        //
        // Summary:
        //     Represents the Boolean greater-than-or-equal-to operator.
        //     To create the Boolean greater-than-or-equal-to operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 >= Field2")
        GreaterOrEqual = 5,
        //
        // Summary:
        //     The LIKE operator. This operator behavior is different, depending on current
        //     circumstances. We recommend that you use StartsWith, Contains and EndsWith function
        //     operators instead of Like, where possible.
        Like = 6,
        //
        // Summary:
        //     Represents the bitwise AND operator.
        //     To create the bitwise AND operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 & 128 = 128")
        BitwiseAnd = 7,
        //
        // Summary:
        //     Represents the bitwise OR operator.
        //     To create the bitwise OR operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 | 3")
        BitwiseOr = 8,
        //
        // Summary:
        //     Represents the bitwise XOR operator.
        //     To create the bitwise XOR operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("(Field1 ^ Field2) = 1")
        BitwiseXor = 9,
        //
        // Summary:
        //     Represents the division operator.
        //     To create the division operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 / Field2 = 2")
        Divide = 10,
        //
        // Summary:
        //     Represents the modulus operator (computes the remainder after dividing its first
        //     operand by its second).
        //     To create the modulus operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 % Field2 = 1")
        Modulo = 11,
        //
        // Summary:
        //     Represents the multiplication operator.
        //     To create the multiplication operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 * Field2 = 100")
        Multiply = 12,
        //
        // Summary:
        //     Represents the addition operator.
        //     To create the addition operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 + Field2 = 20")
        Plus = 13,
        //
        // Summary:
        //     Represents the subtraction operator.
        //     To create the subtraction operator using the DevExpress.Data.Filtering.CriteriaOperator.Parse
        //     method use the following syntax:
        //     CriteriaOperator.Parse("Field1 - Field2 = 10")
        Minus = 14
    }
}
