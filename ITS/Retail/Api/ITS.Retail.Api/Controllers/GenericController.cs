using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ITS.Retail.Api.Controllers
{

    public class GenericController : ODataController
    {
        public EdmEntityObjectCollection Get()
        {
            // Get entity set's EDM type: A collection type.
            /* ODataPath path = Request.ODataProperties().Path;
             IEdmCollectionType collectionType = (IEdmCollectionType)path.EdmType;
             IEdmEntityTypeReference entityType = collectionType.ElementType.AsEntity();

             // Create an untyped collection with the EDM collection type.
             EdmEntityObjectCollection collection =
                 new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType));

             // Add untyped objects to collection.
             DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], entityType, collection);

             return collection;*/
            return null;
        }

    }
}