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
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Linq.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel.Unresolved
{
  [TestFixture]
  public class UnresolvedJoinInfoTest
  {
    private SqlEntityExpression _entityExpression;

    [SetUp]
    public void SetUp ()
    {
      _entityExpression = new SqlEntityDefinitionExpression (typeof (Cook), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Expected a type implementing IEnumerable<T>, but found 'Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain.Cook'.\r\nParameter name: memberInfo")]
    public void Initialization_CardinalityMany_NonEnumerable_Throws ()
    {
      new UnresolvedJoinInfo (_entityExpression, typeof (Cook).GetProperty ("Substitution"), JoinCardinality.Many);
    }

    [Test]
    public void ItemType_CardinalityOne ()
    {
      var joinInfo = new UnresolvedJoinInfo (_entityExpression, typeof (Cook).GetProperty ("Substitution"), JoinCardinality.One);
      Assert.That (joinInfo.ItemType, Is.SameAs (typeof (Cook)));
    }

    [Test]
    public void ItemType_CardinalityMany ()
    {
      _entityExpression = new SqlEntityDefinitionExpression (typeof (Restaurant), "r", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
      var joinInfo = new UnresolvedJoinInfo (_entityExpression, typeof (Restaurant).GetProperty ("Cooks"), JoinCardinality.Many);
      Assert.That (joinInfo.ItemType, Is.SameAs (typeof (Cook)));
    }

    [Test]
    public void Accept ()
    {
      var joinInfo = SqlStatementModelObjectMother.CreateUnresolvedJoinInfo_KitchenCook();

      var joinInfoVisitorMock = MockRepository.GenerateMock<IJoinInfoVisitor>();
      joinInfoVisitorMock.Expect (mock => mock.VisitUnresolvedJoinInfo (joinInfo));

      joinInfoVisitorMock.Replay();

      joinInfo.Accept (joinInfoVisitorMock);
      joinInfoVisitorMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This join has not yet been resolved; call the resolution step first.")]
    public void GetResolvedTableInfo_Throws ()
    {
      var joinInfo = SqlStatementModelObjectMother.CreateUnresolvedJoinInfo_KitchenCook();
      joinInfo.GetResolvedLeftJoinInfo();
    }

    [Test]
    public new void ToString ()
    {
      var joinInfo = SqlStatementModelObjectMother.CreateUnresolvedJoinInfo_KitchenCook ();
      var result = joinInfo.ToString ();

      Assert.That (result, Is.EqualTo ("Kitchen.Cook"));
    }
  }
}