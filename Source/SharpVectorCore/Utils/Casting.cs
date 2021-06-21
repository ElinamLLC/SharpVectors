namespace SharpVectors
{
    /// <summary>
    /// A helper class that converts between types using a combination of implicit and user-defined conversions.
    /// </summary>
    public static class TryCast
    {
        /// <summary>
        /// Converts between types using a combination of implicit and user-defined conversions.
        /// </summary>
        /// <typeparam name="B">The type from which to convert.</typeparam>
        /// <typeparam name="D">The type to which to convert the object.</typeparam>
        /// <param name="baseObject">The object to convert.</param>
        /// <param name="derivedObject">An object whose type at run time is the requested target type.</param>
        /// <returns>This returns <see lanword="true"/> if the conversion is successful; otherwise, <see lanword="false"/>.</returns>
        public static bool Cast<B, D>(B baseObject, out D derivedObject)
            where B : class
            where D : class
        {
            if (baseObject == null)
            {
                derivedObject = null;
                return false;
            }

            derivedObject = baseObject as D;

            return (derivedObject != null);
        }
    }

    /// <summary>
    /// A helper class that safely converts pointers and references to classes up, down, and sideways along the inheritance hierarchy.
    /// </summary>
    public static class DynamicCast
    {
        /// <summary>
        /// Safely converts pointers and references to classes up, down, and sideways along the inheritance hierarchy.
        /// </summary>
        /// <typeparam name="B">The type from which to convert.</typeparam>
        /// <typeparam name="D">The type to which to convert the object.</typeparam>
        /// <param name="baseObject">The object to convert.</param>
        /// <param name="derivedObject">An object whose type at run time is the requested target type.</param>
        /// <returns>This returns <see lanword="true"/> if the conversion is successful; otherwise, <see lanword="false"/>.</returns>
        public static bool Cast<B, D>(B baseObject, out D derivedObject)
            where B : class
            where D : class, B
        {
            if (baseObject == null)
            {
                derivedObject = null;
                return false;
            }

            derivedObject = baseObject as D;

            return (derivedObject != null);
        }
    }
}
