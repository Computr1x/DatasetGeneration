using ExNihilo.Base;
using ExNihilo.Extensions;
using ExNihilo.Visuals;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGen.Generators
{
    internal class CaptchaMaskGenerator
    {
        public static Image Generate(Container container)
        {
            var copyCaptchContainer = container.Copy();
            var visualsWithCaptcha = GetVisualsWithCaptcha(container);
            visualsWithCaptcha.ForEach(x => SetCaptchaMask(x));

            return new Container(container.Size).WithChildren(visualsWithCaptcha).Render();
        }

        private static void SetCaptchaMask(Visual visual)
        {
            if (visual is Container visualContainer)
            {
                foreach (var child in visualContainer.Children)
                    SetCaptchaMask(child);
            }
            else if (visual is Captcha captcha)
            {
                captcha.WithBrush(Color.Magenta);
                captcha.WithType(VisualType.Filled);
            }
        }

        private static List<Visual> GetVisualsWithCaptcha(Container container)
        {
            // TODO remove all colors effects
            List<Visual> visuals = new();

            if(!HasCaptcha(container))
                return visuals;

            foreach (var visual in container.Children)
            {
                if(visual is Container visualContainer)
                {
                    visuals.AddRange(GetVisualsWithCaptcha(visualContainer));
                }
                else if(visual is Captcha)
                {
                    // TODO remove all colors effects
                    visuals.Add(visual);
                }
            }

            if (container.Effects.Any())
            {
                container.Children.Clear();
                container.Children.AddRange(visuals);
                return new List<Visual> { container };
            }
            return visuals;
        }

        private static bool HasCaptcha(Container container)
        {
            foreach (var visual in container.Children)
            {
                if (visual is Container visualContainer)
                {
                    if(HasCaptcha(visualContainer)) 
                        return true;
                }
                else if (visual is Captcha)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
