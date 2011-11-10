﻿// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq.SqlClient;
using System.Linq;
using Remotion.Linq.IntegrationTests.Common.TestDomain.Northwind.CustomTransformers;
using Remotion.Linq.LinqToSqlAdapter;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.SqlBackend.SqlPreparation.MethodCallTransformers;

namespace Remotion.Linq.IntegrationTests.Common.TestDomain.Northwind
{
  /// <summary>
  /// Provides data generated by ReLinq - used by the 101 LinqSamples
  /// </summary>
  public class RelinqNorthwindDataProvider : INorthwindDataProvider
  {
    private readonly NorthwindConnectionManager _manager;
    private readonly NorthwindDataContext _context;
    private readonly MappingResolver _resolver;
    private readonly IQueryResultRetriever _retriever;
    
    private readonly ResultOperatorHandlerRegistry _resultOperatorHandlerRegistry;
    private readonly CompoundMethodCallTransformerProvider _methodCallTransformerProvider;

    private readonly QueryParser _queryParser;
    private readonly IQueryExecutor _executor;

    public RelinqNorthwindDataProvider ()
    {
      _manager = new NorthwindConnectionManager ();
      _context = new NorthwindDataContext (_manager.GetConnectionString ());
      _resolver = new MappingResolver (_context.Mapping);
      _retriever = new QueryResultRetriever (_manager, _resolver);
      
      _resultOperatorHandlerRegistry = ResultOperatorHandlerRegistry.CreateDefault ();

      var methodBasedTransformerRegistry = MethodInfoBasedMethodCallTransformerRegistry.CreateDefault ();
      var nameBasedTransformerRegistry = NameBasedMethodCallTransformerRegistry.CreateDefault ();

      _methodCallTransformerProvider = new CompoundMethodCallTransformerProvider (methodBasedTransformerRegistry, nameBasedTransformerRegistry);
      methodBasedTransformerRegistry.Register (
          typeof (SqlMethods).GetMethod ("Like", new[] { typeof (string), typeof (string) }), 
          new LikeMethodCallTransformer());
      methodBasedTransformerRegistry.Register (DateDiffDayMethodCallTransformer.SupportedMethods, new DateDiffDayMethodCallTransformer());

      foreach (var userDefinedFunction in _context.GetType ().GetMethods ().Where (mi => mi.IsDefined (typeof (FunctionAttribute), false)))
        methodBasedTransformerRegistry.Register (userDefinedFunction, new UserDefinedFunctionTransformer ());

      var customNodeTypeRegistry = new MethodInfoBasedNodeTypeRegistry();
      customNodeTypeRegistry.Register (new[] { typeof (EntitySet<>).GetMethod ("Contains") }, typeof (ContainsExpressionNode));
      
      var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();
      nodeTypeProvider.InnerProviders.Add (customNodeTypeRegistry);
      
      var transformerRegistry = ExpressionTransformerRegistry.CreateDefault ();
      var processor = ExpressionTreeParser.CreateDefaultProcessor (transformerRegistry);
      var expressionTreeParser = new ExpressionTreeParser (nodeTypeProvider, processor);
      _queryParser = new QueryParser (expressionTreeParser);

      _executor = new QueryExecutor (_resolver, _retriever, _resultOperatorHandlerRegistry, _methodCallTransformerProvider, true);
    }

    public IQueryable<Product> Products
    {
      get { return CreateQueryable<Product>(); }
    }

    public IQueryable<Customer> Customers
    {
      get { return CreateQueryable<Customer> (); }
    }

    public IQueryable<Employee> Employees
    {
      get { return CreateQueryable<Employee> (); }
    }

    public IQueryable<Category> Categories
    {
      get { return CreateQueryable<Category> (); }
    }

    public IQueryable<Order> Orders
    {
      get { return CreateQueryable<Order> (); }
    }

    public IQueryable<OrderDetail> OrderDetails
    {
      get { return CreateQueryable<OrderDetail> (); }
    }

    public IQueryable<Contact> Contacts
    {
      get { return CreateQueryable<Contact> (); }
    }

    public IQueryable<Invoices> Invoices
    {
      get { return CreateQueryable<Invoices> (); }
    }

    public IQueryable<QuarterlyOrder> QuarterlyOrders
    {
      get { return CreateQueryable<QuarterlyOrder> (); }
    }

    public IQueryable<Shipper> Shippers
    {
      get { return CreateQueryable<Shipper> (); }
    }

    public IQueryable<Supplier> Suppliers
    {
      get { return CreateQueryable<Supplier> (); }
    }

    public NorthwindDataContext Functions
    {
      get { return _context; }
    }

    public decimal? TotalProductUnitPriceByCategory (int categoryID)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    public decimal? MinUnitPriceByCategory (int? nullable)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    public IQueryable<ProductsUnderThisUnitPriceResult> ProductsUnderThisUnitPrice (decimal @decimal)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    public int CustomersCountByRegion (string wa)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    public ISingleResult<CustomersByCityResult> CustomersByCity (string london)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    public IMultipleResults WholeOrPartialCustomersSet (int p0)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    public IMultipleResults GetCustomerAndOrders (string seves)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    public void CustomerTotalSales (string customerID, ref decimal? totalSales)
    {
      throw new NotImplementedException ("Stored procedures are not relevant for the re-linq SQL backend integration tests.");
    }

    private IQueryable<T> CreateQueryable<T> ()
    {
      return new RelinqQueryable<T> (_queryParser, _executor);
    }
  }
}
