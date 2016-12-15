﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NetworkEmulation.editor {
    public partial class Link : Control, IMarkable, IInitializable {
        private static readonly Pen SelectedPen = new Pen(Color.Black, 5);
        private static readonly Pen DeselectedPen = new Pen(Color.Black, 1);
        private static readonly Pen OnlinePen = new Pen(Color.Green, 1);
        private static readonly Pen OfflinePen = new Pen(Color.Red, 1);
        private readonly NodePictureBox _beginNodePictureBox;
        private readonly NodePictureBox _endNodePictureBox;
        private Pen _pen = DeselectedPen;

        public Link(ref NodePictureBox beginNodePictureBox, ref NodePictureBox endNodePictureBox) {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            _beginNodePictureBox = beginNodePictureBox;
            _endNodePictureBox = endNodePictureBox;


            _beginNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
            _endNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
        }

        public Process Initialize() {
            throw new NotImplementedException();
        }

        public void MarkAsSelected() {
            ChangeStyle(SelectedPen);
        }

        public void MarkAsDeselected() {
            ChangeStyle(DeselectedPen);
        }

        public void MarkAsOnline() {
            ChangeStyle(OnlinePen);
        }

        public void MarkAsOffline() {
            ChangeStyle(OfflinePen);
        }

        private void ChangeStyle(Pen pen) {
            _pen = pen;
            Parent.Refresh();
        }

        public void DrawLink(Graphics graphics) {
            var beginPoint = _beginNodePictureBox.CenterPoint();
            var endPoint = _endNodePictureBox.CenterPoint();


            graphics.DrawLine(_pen, beginPoint, endPoint);
        }

        public bool IsBetween(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            return (_beginNodePictureBox.Equals(beginNodePictureBox) && _endNodePictureBox.Equals(endNodePictureBox)) ||
                   (_beginNodePictureBox.Equals(endNodePictureBox) && _endNodePictureBox.Equals(beginNodePictureBox));
        }
    }
}