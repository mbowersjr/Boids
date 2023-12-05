using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boids.Core.Configuration;
using Boids.Core.Entities;
using Boids.Core.Services;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using static Boids.Core.Gui.Views.FlockListView;
using Num = System.Numerics;

namespace Boids.Core.Gui.Views
{
    public class FlockListView : GuiViewBase, IGuiWindow
    {
        public override string Name => "FlockList";
        public string WindowTitle { get; set; } = "Flock Details";

        private readonly IFlock _flock;
        private readonly GuiManager _guiManager;
        private readonly IGuiFontProvider _fontProvider;
        private readonly ILogger<FlockListView> _logger;

        public FlockListView(IFlock flock, GuiManager guiManager, IGuiFontProvider fontProvider, ILogger<FlockListView> logger)
            : base(guiManager, logger)
        {
            _flock = flock;
            _guiManager = guiManager;
            _fontProvider = fontProvider;
            _logger = logger;
        }
        
        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        
        public override void LoadContent()
        {
            /*
            // Texture loading example

			// First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
			_xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
			{
				var red = (pixel % 300) / 2;
				return new Color(red, 1, 1);
			});

			// Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
			_imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);
            */
            base.LoadContent();
        }

        public override void LayoutView()
        {
            var io = ImGui.GetIO();
            
            // var windowSize = new Num.Vector2(400f, 600f);

            // ImGui.SetNextWindowSize(windowSize, ImGuiCond.Appearing);
            // ImGui.SetNextWindowPos(new Num.Vector2(-10f, 10f), ImGuiCond.Appearing);

            if (!ImGui.Begin(WindowTitle, ref IsVisibleRef))
            {
                ImGui.End();
                return;
            }
            
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Num.Vector2(2f, 2f));

            ImGui.PushFont(_fontProvider.GetFontPtr(GuiFontStyle.RegularLarge));
            ImGui.Text(WindowTitle);
            ImGui.PopFont();

            
            if (ImGui.BeginTable("##split", 2, ImGuiTableFlags.BordersOuter | ImGuiTableFlags.Resizable | ImGuiTableFlags.ScrollY))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("Boid");
                ImGui.TableSetupColumn("Properties");
                ImGui.TableHeadersRow();

                foreach (var boid in _flock.Boids)
                {
                    ShowBoidItem(boid, "Boid", boid.Id);
                }

                ImGui.EndTable();
            }

            ImGui.PopStyleVar();

            ImGui.End();
        }

        private void ShowBoidItem(Boid boid, string prefix, int id)
        {
            ImGui.PushID(id);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var nodeOpen = ImGui.TreeNode($"Boid {id}", $"{prefix}_{id}");
            
            ImGui.TableSetColumnIndex(1);

            if (nodeOpen)
            {
                ShowPropertyRow_Position(boid);
                ShowPropertyRow_Velocity(boid);
                ShowPropertyRow_Rotation(boid);
                
                ImGui.TreePop();
            }

            ImGui.PopID();
        }

        private void ShowPropertyRow_Position(Boid boid)
        {
            ImGui.PushID("Position_Property");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx("Position", flags);

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);
            ImGui.InputFloat2("##value", ref boid.NumPositionRef, null, ImGuiInputTextFlags.ReadOnly);
            
            ImGui.NextColumn();

            ImGui.PopID();

        }
        private void ShowPropertyRow_Velocity(Boid boid)
        {
            ImGui.PushID("Velocity_Property");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx("Velocity", flags);

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);
            ImGui.InputFloat2("##value", ref boid.NumVelocityRef, null, ImGuiInputTextFlags.ReadOnly);
            
            ImGui.NextColumn();

            ImGui.PopID();
        }
        private void ShowPropertyRow_Rotation(Boid boid)
        {
            ImGui.PushID("Rotation_Property");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx("Rotation", flags);

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);
            ImGui.InputFloat("##value", ref boid.RotationRef, 0f, 0f, null, ImGuiInputTextFlags.ReadOnly);
            
            ImGui.NextColumn();

            ImGui.PopID();
        }
    }
}