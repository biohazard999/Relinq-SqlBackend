// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.UnitTests.Linq.Core;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  [TestFixture]
  public class AllResultOperatorHandlerTest
  {
    private ISqlPreparationStage _stageMock;
    private UniqueIdentifierGenerator _generator;
    private AllResultOperatorHandler _handler;
    private SqlStatementBuilder _sqlStatementBuilder;
    private QueryModel _queryModel;
    private SqlPreparationContext _context;

    [SetUp]
    public void SetUp ()
    {
      _stageMock = MockRepository.GenerateMock<ISqlPreparationStage> ();
      _generator = new UniqueIdentifierGenerator ();
      _handler = new AllResultOperatorHandler ();
      _sqlStatementBuilder = new SqlStatementBuilder (SqlStatementModelObjectMother.CreateSqlStatement ())
      {
        DataInfo = new StreamedSequenceInfo (typeof (Cook[]), Expression.Constant (new Cook ()))
      };
      _queryModel = new QueryModel (ExpressionHelper.CreateMainFromClause_Cook (), ExpressionHelper.CreateSelectClause ());
      _context = new SqlPreparationContext ();
    }

    [Test]
    public void HandleResultOperator ()
    {
      var predicate = Expression.Constant (true);
      var preparedPredicate = Expression.Constant (false);
      var resultOperator = new AllResultOperator(predicate);
      var sqlStatement = _sqlStatementBuilder.GetSqlStatement();

      _stageMock
          .Expect (mock => mock.PrepareWhereExpression (
              Arg<Expression>.Matches(e => e.NodeType == ExpressionType.Not && (((UnaryExpression) e).Operand == predicate)), 
              Arg<ISqlPreparationContext>.Matches(c=>c==_context)))
          .Return (preparedPredicate);
      _stageMock.Replay();

      _handler.HandleResultOperator (resultOperator, _queryModel, _sqlStatementBuilder, _generator, _stageMock, _context);

      _stageMock.VerifyAllExpectations ();

      Assert.That (_sqlStatementBuilder.DataInfo, Is.TypeOf (typeof (StreamedScalarValueInfo)));
      Assert.That (((StreamedScalarValueInfo) _sqlStatementBuilder.DataInfo).DataType, Is.EqualTo (typeof (Boolean)));

      var expectedSubStatement = new SqlStatementBuilder (sqlStatement) { WhereCondition = preparedPredicate }.GetSqlStatement ();
      var expectedExistsExpression = new SqlExistsExpression (new SqlSubStatementExpression (expectedSubStatement));
      var expectedExpression = Expression.Not (expectedExistsExpression);

      ExpressionTreeComparer.CheckAreEqualTrees (expectedExpression, _sqlStatementBuilder.SelectProjection);
    }
  }
}