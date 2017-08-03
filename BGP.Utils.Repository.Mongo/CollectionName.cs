using System;

namespace BGP.Utils.Repository.Mongo
{

    /// <summary>
    /// Attribute used to annotate Enities with to override mongo collection name. By default, when this attribute
    /// is not specified, the classname will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CollectionNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the CollectionName class attribute with the desired name.
        /// </summary>
        /// <param name="value">Name of the collection.</param>
        public CollectionNameAttribute(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty collection name is not allowed", "value");
            Name = value;
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string Name { get; private set; }


        /// <summary>
        /// Determines the collection name for TEntity and assures it is not empty
        /// </summary>
        /// <typeparam name="TEntity">The type to determine the collection name for.</typeparam>
        /// <returns>Returns the collection name for TEntity.</returns>
        public static string GetCollectionName<TEntity>() where TEntity : class
        {
            string collectionName;
            collectionName = typeof(TEntity).BaseType.Equals(typeof(object)) ?
                                      GetCollectionNameFromInterface<TEntity>() :
                                      GetCollectionNameFromType<TEntity>();

            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(TEntity).Name;
            }
            return collectionName.ToLowerInvariant();
        }

        /// <summary>
        /// Determines the collection name from the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type to get the collection name from.</typeparam>
        /// <returns>Returns the collection name from the specified type.</returns>
        public static string GetCollectionNameFromInterface<TEntity>() where TEntity : class
        {
            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(typeof(TEntity), typeof(CollectionNameAttribute));

            return att != null ? ((CollectionNameAttribute)att).Name : typeof(TEntity).Name;
        }

        /// <summary>
        /// Determines the collectionname from the specified type.
        /// </summary>
        /// <param name="entitytype">The type of the entity to get the collectionname from.</param>
        /// <returns>Returns the collectionname from the specified type.</returns>
        public static string GetCollectionNameFromType<TEntity>() where TEntity : class
        {
            Type entitytype = typeof(TEntity);
            string collectionname;

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(entitytype, typeof(CollectionNameAttribute));
            if (att != null)
            {
                // It does! Return the value specified by the CollectionName attribute
                collectionname = ((CollectionNameAttribute)att).Name;
            }
            else
            {
                //if (typeof(Entity).IsAssignableFrom(entitytype))
                //{
                //    // No attribute found, get the basetype
                //    while (!entitytype.BaseType.Equals(typeof(Entity)))
                //    {
                //        entitytype = entitytype.BaseType;
                //    }
                //}
                collectionname = entitytype.Name;
            }

            return collectionname;
        }
    }
}
