using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace Boids.Core.Gui
{
    public interface IGuiFontProvider
    {
        ImFontPtr GetFontPtr(GuiFontStyle fontStyle);
        string GetFontFilePath(GuiFontStyle font);
        ImFontPtr AddFont(GuiFontStyle fontStyle, float sizePixels);
        void LoadAllFonts(ImGuiIOPtr io);
    }

    public class GuiFontProvider : IGuiFontProvider
    {
        private readonly Dictionary<GuiFontStyle, ImFontPtr> _fonts = new Dictionary<GuiFontStyle, ImFontPtr>();

        private readonly MainGame _game;
        private readonly ILogger<GuiFontProvider> _logger;

        public GuiFontProvider(MainGame game, ILogger<GuiFontProvider> logger)
        {
            _game = game;
            _logger = logger;
        }

        public ImFontPtr GetFontPtr(GuiFontStyle fontStyle)
        {
            if (_fonts.TryGetValue(fontStyle, out var fontPtr))
            {
                return fontPtr;
            }

            throw new ArgumentException("No font loaded for specified font style.", nameof(fontStyle));
        }

        private static string GetFontResourceName(GuiFontStyle font)
        {
            switch (font)
            {
                case GuiFontStyle.Regular:     
                case GuiFontStyle.RegularLarge:     
                    return "Roboto-Regular";
                case GuiFontStyle.Bold:        
                case GuiFontStyle.BoldLarge:        
                    return "Roboto-Bold";
                case GuiFontStyle.MonoRegular: 
                case GuiFontStyle.MonoRegularLarge: 
                    return "RobotoMono-Regular";
                case GuiFontStyle.MonoBold:    
                case GuiFontStyle.MonoBoldLarge:    
                    return "RobotoMono-Bold";
            }

            throw new InvalidEnumArgumentException(nameof(font), (int)font, typeof(GuiFontStyle));
        }

        public string GetFontFilePath(GuiFontStyle font)
        {
            var fontName = GetFontResourceName(font);
            return GetFontFilePath(fontName);
        }

        private string GetFontFilePath(string fontName)
        {
            if (!fontName.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)) 
                fontName = $"{fontName}.ttf";
            
            var resourceName = $"Fonts\\{fontName}";
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
            var contentDirectory = Path.Combine(assemblyDirectory, _game.Content.RootDirectory);
            var filePath = Path.Combine(contentDirectory, resourceName);

            if (!File.Exists(filePath))
            {
                _logger.LogError("Could not resolve font file path for font \"{FontName}\"", fontName);
                throw new FileNotFoundException($"Font file not found for font \"{fontName}\".", filePath);
            }

            return filePath;
        }

        public ImFontPtr AddFont(GuiFontStyle fontStyle, float sizePixels)
        {
            return AddFont(fontStyle, sizePixels, ImGui.GetIO());
        }

        private ImFontPtr AddFont(GuiFontStyle fontStyle, float sizePixels, ImGuiIOPtr io)
        { 
            var filePath = GetFontFilePath(fontStyle);

            var fontPtr = io.Fonts.AddFontFromFileTTF(filePath, sizePixels);

            _logger.LogInformation("Added font {FontStyle} (Size: {FontSize}px, Path: \"{FilePath}\")", fontStyle, sizePixels, filePath);
            
            return fontPtr;
        }

        const int NormalFontSize = 20;
        const int LargeFontSize = 30;

        public void LoadAllFonts(ImGuiIOPtr io)
        {
            _fonts.Add(GuiFontStyle.Regular,          AddFont(GuiFontStyle.Regular, NormalFontSize, io));
            _fonts.Add(GuiFontStyle.Bold,             AddFont(GuiFontStyle.Bold, NormalFontSize, io));
            _fonts.Add(GuiFontStyle.MonoRegular,      AddFont(GuiFontStyle.MonoRegular, NormalFontSize, io));
            _fonts.Add(GuiFontStyle.MonoBold,         AddFont(GuiFontStyle.MonoBold, NormalFontSize, io));

            _fonts.Add(GuiFontStyle.RegularLarge,     AddFont(GuiFontStyle.RegularLarge, LargeFontSize, io));
            _fonts.Add(GuiFontStyle.BoldLarge,        AddFont(GuiFontStyle.BoldLarge, LargeFontSize, io));
            _fonts.Add(GuiFontStyle.MonoRegularLarge, AddFont(GuiFontStyle.MonoRegularLarge, LargeFontSize, io));
            _fonts.Add(GuiFontStyle.MonoBoldLarge,    AddFont(GuiFontStyle.MonoBoldLarge, LargeFontSize, io));
        }

    }

    public enum GuiFontStyle
    {
        Regular,
        RegularLarge,
        Bold,
        BoldLarge,
        MonoRegular,
        MonoRegularLarge,
        MonoBold,
        MonoBoldLarge
    }

}