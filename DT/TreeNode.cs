using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using System.Drawing;

namespace DT
{
    public class TreeNode<T> where T : IDrawable
    {

        public T value;
        public string branchValue;

        private const float Hoffset = 5;
        private const float Voffset = 30;

        private PointF Center;

        //
        public Font MyFont = null;
        public Pen MyPen = Pens.Black;
        public Brush FontBrush = Brushes.Black;
        public Brush LinkWeightBrush = Brushes.Red;
        public Font LinkWeightFont = null;
        public Brush BgBrush = Brushes.White;
        public Brush WeightBgBrush = SystemBrushes.Control;


        public List<TreeNode<T>> children;

        /*
        public TreeNode(T value)
        {
            this.value = value;
            this.children = new List<TreeNode<T>>();
        }
        public TreeNode(T value, string branchValue)
        {
            this.value = value;
            this.branchValue = branchValue;
            this.children = new List<TreeNode<T>>();
        }
        */


        public TreeNode(T new_data)
            : this(new_data, "")
        {
        }
        public TreeNode(T new_data, string link_weight)
            : this(new_data, link_weight,
                new Font("Tahoma", 12),
                new Font("Tahoma", 8))
        {
        }
        public TreeNode(T new_data, string link_weight, Font fg_font, Font link_weight_font)
        {
            value = new_data;
            branchValue = link_weight;
            MyFont = fg_font;
            LinkWeightFont = link_weight_font;
            children = new List<TreeNode<T>>();
        }


        /*
        public T Value
        {
            get { return this.value; }
            set { this.value = value;}
        }

        public string BranchValue
        {
            get { return this.branchValue; }
            set { this.branchValue = value; }
        }
        */






        public void AddChild(TreeNode<T> child)
        {
            this.children.Add(child);
        }

        public TreeNode<T> GetChild(int index)
        {
            return this.children[index];
        }
        public int ChildrenCount
        {
            get
            {
                return this.children.Count;
            }
        }



        public void Arrange(Graphics gr, ref float xmin, ref float ymin)
        {
            // See how big this node is.
            SizeF my_size = value.GetSize(gr, MyFont);

            // Recursively arrange our children,
            // allowing room for this node.
            float x = xmin;
            float biggest_ymin = ymin + my_size.Height;
            float subtree_ymin = ymin + my_size.Height + Voffset;
            foreach (TreeNode<T> child in children)
            {
                // Arrange this child's subtree.
                float child_ymin = subtree_ymin;
                child.Arrange(gr, ref x, ref child_ymin);

                // See if this increases the biggest ymin value.
                if (biggest_ymin < child_ymin) biggest_ymin = child_ymin;

                // Allow room before the next sibling.
                x += Hoffset;
            }

            // Remove the spacing after the last child.
            if (children.Count > 0) x -= Hoffset;

            // See if this node is wider than the subtree under it.
            float subtree_width = x - xmin;
            if (my_size.Width > subtree_width)
            {
                // Center the subtree under this node.
                // Make the children rearrange themselves
                // moved to center their subtrees.
                x = xmin + (my_size.Width - subtree_width) / 2;
                foreach (TreeNode<T> child in children)
                {
                    // Arrange this child's subtree.
                    child.Arrange(gr, ref x, ref subtree_ymin);

                    // Allow room before the next sibling.
                    x += Hoffset;
                }

                // The subtree's width is this node's width.
                subtree_width = my_size.Width;
            }

            // Set this node's center position.
            Center = new PointF(
                xmin + subtree_width / 2,
                ymin + my_size.Height / 2);

            // Increase xmin to allow room for
            // the subtree before returning.
            xmin += subtree_width;

            // Set the return value for ymin.
            ymin = biggest_ymin;
        }

        // Draw the subtree rooted at this node
        // with the given upper left corner.
        public void DrawTree(Graphics gr, ref float x, float y)
        {
            // Arrange the tree.
            Arrange(gr, ref x, ref y);

            // Draw the tree.
            DrawTree(gr);
        }

        // Draw the subtree rooted at this node.
        public void DrawTree(Graphics gr)
        {
            // Draw the links.
            DrawSubtreeLinks(gr);

            // Draw the nodes.
            DrawSubtreeNodes(gr);
        }

        // Draw the links for the subtree rooted at this node.
        private void DrawSubtreeLinks(Graphics gr)
        {
            foreach (TreeNode<T> child in children)
            {
                // Draw the link between this node this child.
                DrawLink(gr, child.branchValue, Center, child.Center);

                // Recursively make the child draw its subtree nodes.
                child.DrawSubtreeLinks(gr);
            }
        }

        // Draw a link with its weight.
        private void DrawLink(Graphics gr,
            string link_weight, PointF point1, PointF point2)
        {
            // Draw the link.
            gr.DrawLine(MyPen, point1, point2);

            // Display the link's weight.
            if (link_weight != "")
            {
                PointF middle = new PointF(
                    (point1.X + point2.X) / 2f,
                    (point1.Y + point2.Y) / 2f);
                SizeF size = gr.MeasureString(
                    link_weight.ToString(), LinkWeightFont);
                RectangleF rect = new RectangleF(
                    middle.X - size.Width / 2f,
                    middle.Y - size.Height / 2f,
                    size.Width,
                    size.Height);
                gr.FillRectangle(WeightBgBrush, rect);

                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    gr.DrawString(link_weight.ToString(),
                    LinkWeightFont, LinkWeightBrush, rect, sf);
                }
            }
        }

        // Draw the nodes for the subtree rooted at this node.
        private void DrawSubtreeNodes(Graphics gr)
        {
            // Draw this node.
            value.Draw(Center.X, Center.Y, gr, MyPen, BgBrush, FontBrush, MyFont);

            // Recursively make the child draw its subtree nodes.
            foreach (TreeNode<T> child in children)
            {
                child.DrawSubtreeNodes(gr);
            }
        }

        // Return the TreeNode at this point (or null if there isn't one there).
        public TreeNode<T> NodeAtPoint(Graphics gr, PointF target_pt)
        {
            // See if the point is under this node.
            if (value.IsAtPoint(gr, MyFont, Center, target_pt)) return this;

            // See if the point is under a node in the subtree.
            foreach (TreeNode<T> child in children)
            {
                TreeNode<T> hit_node = child.NodeAtPoint(gr, target_pt);
                if (hit_node != null) return hit_node;
            }

            return null;
        }

        // Delete a target node from this node's subtree.
        // Return true if we delete the node.
        public bool DeleteNode(TreeNode<T> target)
        {
            // See if the target is in our subtree.
            foreach (TreeNode<T> child in children)
            {
                // See if it's the child.
                if (child == target)
                {
                    // Delete this child.
                    children.Remove(child);
                    return true;
                }

                // See if it's in the child's subtree.
                if (child.DeleteNode(target)) return true;
            }

            // It's not in our subtree.
            return false;
        }



    }



    /// <summary>Represents a tree data structure</summary>

    /// <typeparam name="T">the type of the values in the

    /// tree</typeparam>

    public class Tree<T> where T : IDrawable
    {
        // The root of the tree
        private TreeNode<T> root;

        //

        public Tree()
        {
            this.root = null;
        }

        public TreeNode<T> Root
        {
            get
            {
                return this.root;
            }
            set
            {
                this.root = value;
            }
        }
        //добавить свойство от бранчей
        public TreeNode<CircleNode> create_id3_tree(DataTable inputs, List<Attribute> attributes, string branchValue)
        {
            TreeNode<CircleNode> desicionTree = new TreeNode<CircleNode>(new CircleNode("")); ;
                        
            if (EntropyFunctions.AllResultsAreTrue(inputs))
            {
                desicionTree.value.Text = "true";
                desicionTree.branchValue = branchValue;
                return desicionTree;                
            }
            if (EntropyFunctions.AllResultsAreFalse(inputs))
            {
                desicionTree.value.Text = "false";
                desicionTree.branchValue = branchValue;
                return desicionTree;
            }
            if (attributes.Count == 0)
            {
                desicionTree.value.Text = EntropyFunctions.MostCommonResult(inputs);
                desicionTree.branchValue = branchValue;
                return desicionTree;
            }

            Attribute bestAttribute = EntropyFunctions.SelectBestAttributeToSplit(attributes, inputs);
            desicionTree.value.Text = bestAttribute.Name;
            desicionTree.branchValue = branchValue;

            for (int i = 0; i < bestAttribute.Values.Count; i++)
            {
                DataTable subset = EntropyFunctions.SelectDataByAttributeAndValues(bestAttribute, inputs, bestAttribute.Values[i]);
                if (subset.Rows.Count != 0)
                {
                    attributes.Remove(bestAttribute);
                    desicionTree.AddChild(create_id3_tree(subset, attributes, bestAttribute.Values[i]));
                    
                }
            }
            return desicionTree;
        }
                   
        
        /// <summary>Traverses and prints tree in
        /// Depth-First Search (DFS) manner</summary>
        /// <param name="root">the root of the tree to be
        /// traversed</param>
        /// <param name="spaces">the spaces used for
        /// representation of the parent-child relation</param>
        
        private void PrintDFS(TreeNode<T> root, string spaces)
        {
            if (this.root == null)
            {
                return;
            }
            //Console.WriteLine(spaces + root.Value);
            string branch = (root.branchValue.ToString() != "") ? "-" + root.branchValue + "?->" + "\n" + spaces + "   ": "";
            Global.RichTextBox.Text += spaces + branch + "[" + root.value + "]" + "\n";

            TreeNode<T> child = null;
            for (int i = 0; i < root.ChildrenCount; i++)
            {
                child = root.GetChild(i);
                PrintDFS(child, spaces + "   ");
            }
        }
        


        /// <summary>Traverses and prints the tree in
        /// Depth-First Search (DFS) manner</summary>
        
        public void TraverseDFS()
        {
            this.PrintDFS(this.root, string.Empty);
        }

        

    }
}
