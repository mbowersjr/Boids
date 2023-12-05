using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Num = System.Numerics;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using ImGuiNET;
using Boids.Core.Behaviors;
using Boids.Core.Entities;

namespace Boids.Core.Gui.Views
{
    public class BehaviorsView : GuiViewBase, IGuiWindow
    {
        public override string Name { get; } = "Behaviors";

        public string WindowTitle { get; set; } = "Behaviors";

        private readonly IFlockBehaviors _behaviors;
        private readonly IGuiFontProvider _fontProvider;
        private readonly ILogger<BehaviorsView> _logger;

        public BehaviorsView(IFlockBehaviors behaviors, GuiManager guiManager, IGuiFontProvider fontProvider, ILogger<BehaviorsView> logger)
            : base(guiManager, logger)
        {
            _behaviors = behaviors;
            _fontProvider = fontProvider;
        }

        public override void LayoutView()
        {
            var windowSize = new Num.Vector2(400f, 600f);

            ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Num.Vector2(-10f, 10f), ImGuiCond.FirstUseEver);

            if (!ImGui.Begin(WindowTitle, ref IsVisibleRef))
            {
                ImGui.End();
                return;
            }
            
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Num.Vector2(2f, 2f));

            ImGui.PushFont(_fontProvider.GetFontPtr(GuiFontStyle.RegularLarge));
            ImGui.Text(WindowTitle);
            ImGui.PopFont();

            var tableFlags = ImGuiTableFlags.BordersV | 
                             ImGuiTableFlags.BordersOuterH | 
                             ImGuiTableFlags.Resizable | 
                             ImGuiTableFlags.RowBg | 
                             ImGuiTableFlags.NoBordersInBody;

            if (ImGui.BeginTable("BehaviorsTable", 2, tableFlags))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.NoHide, GuiManager.TextBaseWidth * GuiManager.TextBaseWidth);
                ImGui.TableSetupColumn("Properties", ImGuiTableColumnFlags.WidthFixed, GuiManager.TextBaseWidth * 25);
                ImGui.TableHeadersRow();

                for (var i = 0; i < _behaviors.Behaviors.Count; i++)
                {
                    var behavior = _behaviors.Behaviors[i];
                    ShowBehaviorItem(behavior);
                }

                ImGui.EndTable();
            }

            ImGui.PopStyleVar();

            ImGui.End();
        }

        private void ShowBehaviorItem(IBehavior behavior)
        {
            ImGui.PushID(behavior.Name);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var nodeOpen = ImGui.TreeNode(behavior.Name, $"Behavior_{behavior.Name}");
            
            ImGui.TableSetColumnIndex(1);

            if (nodeOpen)
            {
                ShowPropertyRow_Enabled(behavior);
                ShowPropertyRow_Coefficient(behavior);
                ShowPropertyRow_Radius(behavior);
                ShowPropertyRow_Order(behavior);
                
                ImGui.TreePop();
            }

            ImGui.PopID();
        }

        private void ShowPropertyRow_Enabled(IBehavior behavior)
        {
            ImGui.PushID("Property_Enabled");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx("Enabled", flags);

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);
            ImGui.Checkbox("##value", ref behavior.EnabledRef);
                
            
            ImGui.NextColumn();

            ImGui.PopID();
        }

        private void ShowPropertyRow_Radius(IBehavior behavior)
        {
            ImGui.PushID("Property_Radius");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx("Radius", flags);

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);

            var inputFlags = ImGuiInputTextFlags.ReadOnly;
            ImGui.InputFloat("##value", ref behavior.RadiusRef, 1f, 5f, null, inputFlags);

            ImGui.NextColumn();

            ImGui.PopID();
        }

        private void ShowPropertyRow_Coefficient(IBehavior behavior)
        {
            ImGui.PushID("Property_Coefficient");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx("Coefficient", flags);

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);

            var inputFlags = ImGuiInputTextFlags.ReadOnly;
            ImGui.InputFloat("##value", ref behavior.CoefficientRef, 1f, 5f, null, inputFlags);
                
            ImGui.NextColumn();

            ImGui.PopID();
        }
        
        private void ShowPropertyRow_Order(IBehavior behavior)
        {
            ImGui.PushID("Property_Order");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();

            var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
            ImGui.TreeNodeEx("Order", flags);

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(-ImGuiRenderer.FLT_MIN);

            var inputFlags = ImGuiInputTextFlags.ReadOnly;
            ImGui.InputInt("##value", ref behavior.OrderRef, 1, 1, inputFlags);
                
            ImGui.NextColumn();

            ImGui.PopID();
        }
        

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
