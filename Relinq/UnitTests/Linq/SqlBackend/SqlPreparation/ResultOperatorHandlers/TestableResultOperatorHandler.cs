// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Linq.UnitTests.Linq.Core.Clauses.ResultOperators;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;
using Remotion.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  public class TestableResultOperatorHandler : ResultOperatorHandler<TestChoiceResultOperator>
  {
    public override void HandleResultOperator (TestChoiceResultOperator resultOperator, SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage, ISqlPreparationContext context)
    {
      throw new NotImplementedException();
    }

    public new void EnsureNoTopExpression (
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationStage stage,
        ISqlPreparationContext context)
        
    {
      base.EnsureNoTopExpression (sqlStatementBuilder, generator, stage, context);
    }

    public new void EnsureNoGroupExpression (
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationStage stage,
        ISqlPreparationContext context)
    {
      base.EnsureNoGroupExpression (sqlStatementBuilder, generator, stage, context);
    }

    public new void EnsureNoDistinctQuery (
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationStage stage,
        ISqlPreparationContext context)
    {
      base.EnsureNoDistinctQuery(sqlStatementBuilder, generator, stage, context);
    }

    public new void MoveCurrentStatementToSqlTable (
       SqlStatementBuilder sqlStatementBuilder,
       UniqueIdentifierGenerator generator,
       ISqlPreparationContext context,
       Func<ITableInfo, SqlTableBase> tableGenerator, ISqlPreparationStage stage)
    {
      base.MoveCurrentStatementToSqlTable (sqlStatementBuilder, generator, context, tableGenerator, stage);
    }

    public new void UpdateDataInfo (ResultOperatorBase resultOperator, SqlStatementBuilder sqlStatementBuilder, IStreamedDataInfo dataInfo)
    {
      base.UpdateDataInfo (resultOperator, sqlStatementBuilder, dataInfo);
    }
  }
}