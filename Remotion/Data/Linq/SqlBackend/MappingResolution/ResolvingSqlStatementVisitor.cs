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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="ResolvingSqlStatementVisitor"/> implements <see cref="SqlStatementVisitorBase"/>.
  /// </summary>
  public class ResolvingSqlStatementVisitor : SqlStatementVisitorBase
  {
    private readonly ISqlStatementResolver _resolver;

    public ResolvingSqlStatementVisitor (ISqlStatementResolver resolver)
    {
      ArgumentUtility.CheckNotNull ("resolver", resolver);

      _resolver = resolver;
    }

    protected override Expression VisitSelectProjection (Expression selectProjection, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("selectProjection", selectProjection);
      ArgumentUtility.CheckNotNull ("generator", generator);

      return ResolvingExpressionVisitor.ResolveExpressions (selectProjection, _resolver, generator);
    }

    protected override void VisitSqlTable (SqlTable sqlTable)
    {
      ArgumentUtility.CheckNotNull ("sqlTable", sqlTable);
      
      ResolvingTableSourceVisitor.ResolveTableSource (sqlTable, _resolver);
    }
  }
}