using Civilization_draft.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Civilization_draft.Models
{
    static class DataAccess
    {
        public static class Json
        {
            private static readonly string path = AppDomain.CurrentDomain.BaseDirectory + "CivData/";
            private static readonly string configFilepath = AppDomain.CurrentDomain.BaseDirectory + "SavedConfig.json";

            public static SortedList<string, Dlc> LoadDlc()
            {
                List<Dlc> listFromJson;
                using (StreamReader r = new StreamReader(path + "Dlc.json"))
                {
                    string dlc = r.ReadToEnd();
                    listFromJson = new JavaScriptSerializer().Deserialize<List<Dlc>>(dlc);
                }

                SortedList<string, Dlc> outputList = new SortedList<string, Dlc>();
                foreach (var dlc in listFromJson)
                {
                    outputList.Add(dlc.Abbreviation, dlc);
                }

                return outputList;
            }

            public static List<Civilization> LoadCivilizations()
            {
                List<Civilization> civList;
                using (StreamReader r = new StreamReader(path + "Civilizations.json"))
                {
                    string jsonCivilizations = r.ReadToEnd();
                    civList = new JavaScriptSerializer().Deserialize<List<Civilization>>(jsonCivilizations);
                }

                return civList.OrderBy(c => c.Name).ToList();
            }

            public static void SaveConfig(Config config)
            {
                var json = new JavaScriptSerializer().Serialize(config);

                StreamWriter writer = new StreamWriter(configFilepath, false);
                using (writer)
                {
                    writer.Write(json);
                }
            }
            public static Config LoadConfig()
            {
                if (!File.Exists(configFilepath))
                    return null;

                using (StreamReader r = new StreamReader(configFilepath))
                {
                    string jsonString = r.ReadToEnd();
                    return new JavaScriptSerializer().Deserialize<Config>(jsonString);
                }
            }
        }

        public static class ClipBoard
        {
            public static void CopyResultAsText(List<PlayerResult> result)
            {
                var formattedResult = result.Select(playerResult => new ClipboardPlayerResult
                {
                    Player = playerResult.PlayerNumber,
                    Civilizations = playerResult.Civs.Select(civButton => new CivSimple
                    {
                        Civ = civButton.Civ.Name,
                        Leader = civButton.Civ.Leader
                    }).ToList()
                }).ToList();

                string text = "";
                foreach (var playerResult in formattedResult)
                {
                    text +=
                        "--- Player " + playerResult.Player + "---";
                    foreach (var civ in playerResult.Civilizations)
                    {
                        text +=
                            "\n" + civ.Civ
                            + "\n" + civ.Leader + "\n";
                    }
                    text += "\n\n";
                }

                Clipboard.SetText(text);
            }
            public static void CopyUiElementAsImage(UIElement uiElement)
            {
                // http://miteshsureja.blogspot.com/2016/07/how-to-capture-screenshot-of-wpf.html

                Size size = uiElement.RenderSize;
                double width = size.Width;
                double height = size.Height;

                // Create drawing of the uiElement to caputure content not visible
                // RenderTargetBitmap can render the UiElement directly,
                // but content not visible will not be rendered correctly
                DrawingVisual drawingOfUiElement = new DrawingVisual();
                using (DrawingContext drawingContext = drawingOfUiElement.RenderOpen())
                {
                    var visualBrush = new VisualBrush(uiElement);
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(width, height)));
                }

                // Make a white background, otherwise the background will be black because of problems with opacity
                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/a6972b7f-5ccb-422d-b203-134ef9f10084/how-to-capture-entire-usercontrol-image-to-clipboard?forum=wpf
                Rectangle whiteBackground = new Rectangle()
                {
                    Width = width,
                    Height = height,
                    Fill = Brushes.White
                };
                whiteBackground.Measure(size);
                whiteBackground.Arrange(new Rect(size));

                // Create an image
                RenderTargetBitmap imageBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);
                imageBitmap.Render(whiteBackground);
                imageBitmap.Render(drawingOfUiElement);

                Clipboard.SetImage(imageBitmap);
            }
        }
    }
}
