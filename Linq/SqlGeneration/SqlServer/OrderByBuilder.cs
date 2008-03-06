using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.Linq.Clauses;
using Rubicon.Data.Linq.DataObjectModel;
using Rubicon.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.Linq.SqlGeneration.SqlServer
{
  public class OrderByBuilder : IOrderByBuilder
  {
    private static string GetOrderedDirectionString (OrderDirection direction)
    {
      switch (direction)
      {
        case OrderDirection.Asc:
          return "ASC";
        case OrderDirection.Desc:
          return "DESC";
        default:
          throw new NotSupportedException ("OrderDirection " + direction + " is not supported.");
      }
    }

    private readonly ICommandBuilder _commandBuilder;

    public OrderByBuilder (ICommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
    }

    public void BuildOrderByPart (List<OrderingField> orderingFields)
    {
      if (orderingFields.Count != 0)
      {
        _commandBuilder.Append (" ORDER BY ");
        _commandBuilder.AppendSeparatedItems (orderingFields, AppendOrderingField);
      }
    }

    private void AppendOrderingField (OrderingField orderingField)
    {
      _commandBuilder.AppendColumn (orderingField.Column);
      _commandBuilder.Append (" ");
      _commandBuilder.Append (GetOrderedDirectionString (orderingField.Direction));
    }
  }
}