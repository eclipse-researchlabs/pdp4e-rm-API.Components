//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.Assets.Models;
//using Core.Database;
//using Core.Database.Enums;
//using Core.Database.QueryLanguages;
//using Core.Database.Tables;
//using GraphQL;
//using GraphQL.EntityFramework;
//using GraphQL.Types;

//namespace Core.Api.Controllers
//{
//    public class Schema : GraphQL.Types.Schema
//    {
//        public Schema(IDependencyResolver resolver) : base(resolver)
//        {
//            Query = resolver.Resolve<Query>();
//            //Subscription = resolver.Resolve<Subscription>();
//        }
//    }

//    public class Query : QueryGraphType<BeawreContext>
//    {
//        public Query(IEfGraphQLService<BeawreContext> efGraphQlService) : base(efGraphQlService)
//        { 
//            AddQueryField(
//                name: "users",
//                resolve: context => context.DbContext.User.Where(x => !x.IsDeleted)
//                );

//            Field<ListGraphType<AssetGraphQl>>(
//                name: "groups",
//                resolve: context =>
//                {
//                    var dbContext = (BeawreContext)context.UserContext;
//                    var relationships = dbContext.Relationship.Where(x => x.FromType == ObjectType.AssetGroup && !x.IsDeleted).Select(x => x.FromId).ToArray();
//                    return dbContext.Assets.Where(x => relationships.Contains(x.Id) && !x.IsDeleted && x.IsGroup).ToList();
//                });

//            AddQueryField(
//                name: "containers",
//                resolve: context => context.DbContext.Container.Where(x => !x.IsDeleted)
//            );

//            AddQueryField(
//                name: "assets",
//                resolve: context => context.DbContext.Assets.Where(x => !x.IsDeleted)
//            );

//            AddQueryField(
//                name: "edges",
//                resolve: context => context.DbContext.Relationship.Where(x => x.FromType == ObjectType.Asset && x.ToType == ObjectType.Asset).Where(x => !x.IsDeleted)
//            );
//        }
//    }
//}
