using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleRDP {
    public enum TreeDropPosition {
        None,
        Above,
        Below,
        Into,
        AppendToRoot
    }

    public class DraggableTreeView : TreeView {
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        private const int WM_PAINT = 0x000F;
        private const int WM_VSCROLL = 0x0115;
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private TreeNode? _dropTargetNode;
        private TreeDropPosition _dropPosition = TreeDropPosition.None;
        private bool _dropAtChildIndent = false;
        private DateTime _lastScrollTime = DateTime.MinValue;

        public TreeNode? DropTargetNode => _dropTargetNode;
        public TreeDropPosition DropPosition => _dropPosition;

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
        }

        public void SetDropIndicator(TreeNode? targetNode, TreeDropPosition position, bool childIndent = false) {
            if (_dropTargetNode == targetNode && _dropPosition == position && _dropAtChildIndent == childIndent) {
                return;
            }
            _dropTargetNode = targetNode;
            _dropPosition = position;
            _dropAtChildIndent = childIndent;
            this.Invalidate();
        }

        public void ClearDropIndicator() {
            if (_dropPosition == TreeDropPosition.None && _dropTargetNode == null) {
                return;
            }
            _dropTargetNode = null;
            _dropPosition = TreeDropPosition.None;
            _dropAtChildIndent = false;
            this.Invalidate();
        }

        public void ScrollIfNeeded(Point clientPoint) {
            var now = DateTime.UtcNow;
            if ((now - _lastScrollTime).TotalMilliseconds < 120) {
                return;
            }

            const int scrollMargin = 24;
            if (clientPoint.Y < scrollMargin) {
                SendMessage(this.Handle, WM_VSCROLL, (IntPtr)SB_LINEUP, IntPtr.Zero);
                _lastScrollTime = now;
            } else if (clientPoint.Y > this.ClientSize.Height - scrollMargin) {
                SendMessage(this.Handle, WM_VSCROLL, (IntPtr)SB_LINEDOWN, IntPtr.Zero);
                _lastScrollTime = now;
            }
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && _dropPosition != TreeDropPosition.None) {
                DrawDropIndicator();
            }
        }

        private void DrawDropIndicator() {
            using (var g = this.CreateGraphics()) {
                var accentColor = Color.FromArgb(0, 120, 215);

                if (_dropPosition == TreeDropPosition.Into && _dropTargetNode != null) {
                    var bounds = _dropTargetNode.Bounds;
                    if (bounds.Height <= 0) {
                        return;
                    }
                    var rowRect = new Rectangle(2, bounds.Top + 1, Math.Max(10, this.ClientSize.Width - 5), bounds.Height - 2);
                    using (var fillBrush = new SolidBrush(Color.FromArgb(35, accentColor)))
                    using (var borderPen = new Pen(accentColor, 2)) {
                        g.FillRectangle(fillBrush, rowRect);
                        g.DrawRectangle(borderPen, rowRect);
                    }
                    return;
                }

                int y = -1;
                int leftX = 4;

                if (_dropPosition == TreeDropPosition.Above && _dropTargetNode != null) {
                    var bounds = _dropTargetNode.Bounds;
                    if (bounds.Height <= 0) {
                        return;
                    }
                    y = Math.Max(1, bounds.Top);
                    if (_dropAtChildIndent) {
                        leftX = Math.Max(22, bounds.Left - 20);
                    }
                } else if (_dropPosition == TreeDropPosition.Below && _dropTargetNode != null) {
                    var bounds = _dropTargetNode.Bounds;
                    if (bounds.Height <= 0) {
                        return;
                    }
                    y = Math.Min(this.ClientSize.Height - 2, bounds.Bottom - 1);
                    if (_dropAtChildIndent) {
                        leftX = Math.Max(22, bounds.Left - 20);
                    }
                } else if (_dropPosition == TreeDropPosition.AppendToRoot) {
                    TreeNode? lastVisible = GetLastVisibleNode();
                    y = lastVisible != null
                        ? Math.Min(this.ClientSize.Height - 2, lastVisible.Bounds.Bottom)
                        : 2;
                    leftX = 4;
                }

                if (y >= 0) {
                    int rightX = Math.Max(leftX + 10, this.ClientSize.Width - 6);
                    using (var pen = new Pen(accentColor, 2)) {
                        g.DrawLine(pen, leftX, y, rightX, y);
                        // Small end-caps for clear insertion cue
                        g.DrawLine(pen, leftX, Math.Max(0, y - 3), leftX, Math.Min(this.ClientSize.Height - 1, y + 3));
                        g.DrawLine(pen, rightX, Math.Max(0, y - 3), rightX, Math.Min(this.ClientSize.Height - 1, y + 3));
                    }
                }
            }
        }

        private TreeNode? GetLastVisibleNode() {
            if (this.Nodes.Count == 0) {
                return null;
            }
            TreeNode node = this.Nodes[this.Nodes.Count - 1];
            while (node.IsExpanded && node.Nodes.Count > 0) {
                node = node.Nodes[node.Nodes.Count - 1];
            }
            return node;
        }
    }
}
