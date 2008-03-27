using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rubicon.Data.Linq.DataObjectModel;
using Rubicon.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.Linq.SqlGeneration.SqlServer
{
  public class FromBuilder : IFromBuilder
  {
    private readonly ICommandBuilder _commandBuilder;

    public FromBuilder (ICommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
    }

    public void BuildFromPart (List<IFromSource> fromSources, JoinCollection joins)
    {
      _commandBuilder.Append ("FROM ");

      IEnumerable<string> tableEntries = CombineTables (fromSources, joins);
      _commandBuilder.Append (SeparatedStringBuilder.Build (", ", tableEntries));
    }

    private IEnumerable<string> CombineTables (IEnumerable<IFromSource> fromSources, JoinCollection joins)
    {
      foreach (Table table in fromSources)
        yield return SqlServerUtility.GetTableDeclaration (table) + BuildJoinPart (joins[table]);
    }

    private string BuildJoinPart (IEnumerable<SingleJoin> joins)
    {
      StringBuilder joinStatement = new StringBuilder ();
      foreach (SingleJoin join in joins)
        AppendJoinExpression (joinStatement, join);
      return joinStatement.ToString ();
    }

    private void AppendJoinExpression (StringBuilder joinStatement, SingleJoin join)
    {
      joinStatement.Append (" LEFT OUTER JOIN ")
          .Append (SqlServerUtility.GetTableDeclaration ((Table) join.RightSide))
          .Append (" ON ")
          .Append (SqlServerUtility.GetColumnString (join.LeftColumn))
          .Append (" = ")
          .Append (SqlServerUtility.GetColumnString (join.RightColumn));
    }
  }
}