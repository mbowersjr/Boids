using Boids.Core.Entities;
using Boids.Core.Services;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Boids.Core.Gui.Views
{
    public class FlockListView : GuiViewBase, IGuiWindow
    {
        public string WindowTitle { get; set; } = "Flock";
        
        private IFlock _flock;

        public FlockListView(GuiManager guiManager, IFlock flock, ILogger<FlockListView> logger)
            : base(guiManager, logger)
        {
            _flock = flock;
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
            
            // var windowSize = new Num.Vector2(400, 600);
            // var viewportWidth = ImGui.GetContentRegionAvail().X;

            //ImGui.SetNextWindowDockID(GuiManager.DockSpaceId, ImGuiCond.FirstUseEver);
            var viewport = ImGui.GetMainViewport();

            var windowSize = new Num.Vector2(400f, 600f);

            ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Num.Vector2(-windowSize.X, 10f), ImGuiCond.FirstUseEver);

            if (!ImGui.Begin(WindowTitle, ref IsVisibleRef))
            {
                ImGui.End();
                return;
            }
            
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Num.Vector2(2f, 2f));

            if (ImGui.BeginTable("##split", 2, ImGuiTableFlags.BordersOuter | ImGuiTableFlags.Resizable | ImGuiTableFlags.ScrollY))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("Boid");
                ImGui.TableSetupColumn("Properties");
                ImGui.TableHeadersRow();

                for (var i = 0; i < _flock.Boids.Count; i++)
                {
                    var boid = _flock.Boids[i];

                    ShowBoidItem(boid, "Boid", i);
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

            var nodeOpen = ImGui.TreeNode("Boid", $"{prefix}_{id}");
            ImGui.TableSetColumnIndex(1);

            if (nodeOpen)
            {
                ImGui.PushID("Position");

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

                
                ImGui.PushID("Velocity");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.AlignTextToFramePadding();
                ImGui.TreeNodeEx("Velocity", flags);

                ImGui.TableSetColumnIndex(1);
                ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);
                ImGui.InputFloat2("##value", ref boid.NumVelocityRef, null, ImGuiInputTextFlags.ReadOnly);
                ImGui.NextColumn();

                ImGui.PopID();
                
                ImGui.TreePop();
            }

            ImGui.PopID();
        }
    }
}