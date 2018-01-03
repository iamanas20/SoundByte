/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    ///     Helpers Composition Classes
    /// </summary>
    public static class CompositionHelper
    {
        /// <summary>
        ///     Adds a drop shadow to an element
        /// </summary>
        /// <param name="element">The element to apply the effec to</param>
        /// <param name="shadowOffset">Offset of the shadow</param>
        /// <param name="shadowBlurRadius">Blur radius of the shadow</param>
        /// <param name="shadowColor">Color of the shadow</param>
        /// <param name="shadowHost">Item the shadow effect will be attached to</param>
        public static void CreateElementShadow(this UIElement element, Vector3 shadowOffset, float shadowBlurRadius,
            Color shadowColor, UIElement shadowHost)
        {
            // Get the compositor for this element
            var compositor = ElementCompositionPreview.GetElementVisual(element).Compositor;

            // Create a visual element that we will use to attach the shadow
            var spriteVisual = compositor.CreateSpriteVisual();

            // Create the shadow effect
            var shadow = compositor.CreateDropShadow();
            shadow.Offset = shadowOffset;
            shadow.BlurRadius = shadowBlurRadius;
            shadow.Color = shadowColor;

            // Apply the effect
            spriteVisual.Shadow = shadow;
            ElementCompositionPreview.SetElementChildVisual(shadowHost, spriteVisual);

            // Get the object visual for animation
            var elementVisual = ElementCompositionPreview.GetElementVisual(element);

            // Create the animation
            var sizeAnimation = compositor.CreateExpressionAnimation("ElementVisual.Size");
            sizeAnimation.SetReferenceParameter("ElementVisual", elementVisual);

            // Start the size animation
            spriteVisual.StartAnimation("Size", sizeAnimation);
        }
    }
}