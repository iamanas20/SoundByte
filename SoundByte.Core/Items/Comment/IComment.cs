namespace SoundByte.Core.Items.Comment
{
    /// <summary>
    ///     Extend custom service comment classes
    ///     off of this interface.
    /// </summary>
    public interface IComment
    {
        /// <summary>
        ///     Convert the service specific comment implementation to a
        ///     universal implementation. Overide this method and provide
        ///     the correct mapping between the service specific and universal
        ///     classes.
        /// </summary>
        /// <returns>A base comment item.</returns>
        BaseComment ToBaseComment();
    }
}
