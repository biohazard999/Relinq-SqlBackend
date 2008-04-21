using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Data.Linq.DataObjectModel;
using Rubicon.Data.Linq.Parsing;
using Rubicon.Utilities;

namespace Rubicon.Data.Linq.SqlGeneration.SqlServer
{
  public class SqlServerEvaluationVisitor : IEvaluationVisitor
  {

    public SqlServerEvaluationVisitor (CommandBuilder commandBuilder, IDatabaseInfo databaseInfo)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("databaseInfo", databaseInfo);
      
      CommandBuilder = commandBuilder;
      DatabaseInfo = databaseInfo;
    }

    public CommandBuilder CommandBuilder { get; private set; }
    public IDatabaseInfo DatabaseInfo { get; private set; }


    public void VisitBinaryEvaluation (BinaryEvaluation binaryEvaluation)
    {
      ArgumentUtility.CheckNotNull ("binaryEvaluation", binaryEvaluation);
      CommandBuilder.Append ("(");
      binaryEvaluation.Left.Accept (this);
      switch (binaryEvaluation.Kind)
      {
        case BinaryEvaluation.EvaluationKind.Add:
          CommandBuilder.Append (" + ");
          break;
        case BinaryEvaluation.EvaluationKind.Divide:
          CommandBuilder.Append (" / ");
          break;
        case BinaryEvaluation.EvaluationKind.Modulo:
          CommandBuilder.Append (" % ");
          break;
        case BinaryEvaluation.EvaluationKind.Multiply:
          CommandBuilder.Append (" * ");
          break;
        case BinaryEvaluation.EvaluationKind.Subtract:
          CommandBuilder.Append (" - ");
          break;
      }
      binaryEvaluation.Right.Accept (this);
      CommandBuilder.Append (")");

    }

    public void VisitComplexCriterion (ComplexCriterion complexCriterion)
    {
      ArgumentUtility.CheckNotNull ("complexCriterion", complexCriterion);
      CommandBuilder.Append ("(");
      complexCriterion.Left.Accept (this);
      switch (complexCriterion.Kind)
      {
        case ComplexCriterion.JunctionKind.And:
          CommandBuilder.Append (" AND ");
          break;
        case ComplexCriterion.JunctionKind.Or:
          CommandBuilder.Append (" OR ");
          break;
      }
      complexCriterion.Right.Accept(this);
      CommandBuilder.Append(")");
    }

    public void VisitNotCriterion (NotCriterion notCriterion)
    {
      ArgumentUtility.CheckNotNull ("notCriterion", notCriterion);
      CommandBuilder.Append (" NOT ");
      notCriterion.NegatedCriterion.Accept (this);
    }

   public void VisitConstant (Constant constant)
    {
      ArgumentUtility.CheckNotNull ("constant", constant);
      if (constant.Value == null)
        CommandBuilder.CommandText.Append ("NULL");
      else if (constant.Value.Equals (true))
        CommandBuilder.Append ("(1=1)");
      else if (constant.Value.Equals (false))
        CommandBuilder.Append ("(1<>1)");
      else
      {
        CommandBuilder commandBuilder = new CommandBuilder (CommandBuilder.CommandText,CommandBuilder.CommandParameters, DatabaseInfo);
        CommandParameter parameter = commandBuilder.AddParameter (constant.Value);
        CommandBuilder.CommandText.Append (parameter.Name);
      }
    }

    public void VisitColumn (Column column)
    {
      ArgumentUtility.CheckNotNull ("column", column);
      CommandBuilder.CommandText.Append (SqlServerUtility.GetColumnString (column));
    }

    public void VisitBinaryCondition (BinaryCondition binaryCondition)
    {
      ArgumentUtility.CheckNotNull ("binaryCondition", binaryCondition);
      new BinaryConditionBuilder (CommandBuilder, DatabaseInfo).BuildBinaryConditionPart (binaryCondition);
    }

    public void VisitSubQuery (SubQuery subQuery)
    {
      CommandBuilder.Append ("((");
      new SqlServerGenerator (subQuery.QueryModel, DatabaseInfo,CommandBuilder,ParseContext.SubQueryInSelect).BuildCommandString();
      CommandBuilder.Append (") ");
      CommandBuilder.Append (subQuery.Alias);
      CommandBuilder.Append (")");
    }

    public void VisitMethodCallEvaluation (MethodCallEvaluation methodCallEvaluation)
    {
      switch (methodCallEvaluation.EvaluationMethodInfo.Name)
      {
        case "ToUpper":
          CommandBuilder.Append ("UPPER(");
          methodCallEvaluation.EvaluationParameter.Accept (this);
          CommandBuilder.Append (")");
          break;
        case "Remove":
          CommandBuilder.Append ("STUFF(");
          methodCallEvaluation.EvaluationParameter.Accept (this);
          CommandBuilder.Append (",");
          foreach (var argument in methodCallEvaluation.EvaluationArguments)
          {
            argument.Accept(this);
          }
          CommandBuilder.Append (",CONVERT(Int,DATALENGTH(");
          methodCallEvaluation.EvaluationParameter.Accept (this);
          CommandBuilder.Append(") / 2), \"");
          CommandBuilder.Append (")");
          break;
      }
    }
  }
}