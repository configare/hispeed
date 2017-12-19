using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    public class ElementsEditor
    {
        /// <summary>
        /// 将元素编组
        /// </summary>
        /// <param name="elements">需要编组的元素</param>
        /// <param name="host">当前的layoutHost</param>
        /// <returns></returns>
        public static IElementGroup ElementsToGroup(IElement[] elements, ILayoutHost host)
        {
            if (elements == null || elements.Length == 0)
                return null;
            int count = elements.Length;
            IElementGroup group = new ElementGroup();
            IElement[] parents = new IElement[count];
            IElement parent;
            //将所选element从其父节点中移除
            for (int i = 0; i < count; i++)
            {
                parent = host.LayoutRuntime.Layout.FindParent(elements[i]);
                //parent = group.FindParent(elements[i]);
                if (parent == null)
                    return null;
                parents[i] = parent;
                (parents[i] as IElementGroup).Elements.Remove(elements[i]);
                group.Elements.Add(elements[i]);
            }

            //统计所选element的父节点的深度级数
            int[] levels = new int[count];
            IElement[] parentClon = parents.Clone() as IElement[];
            for (int i = 0; i < count; i++)
            {
                while (parentClon[i] != null)
                {
                    parentClon[i] = (parentClon[i] as IElementGroup).FindParent(parentClon[i]);
                    levels[i]++;
                }
            }
            //查找深度最大的父节点
            int maxLevelIndex = 0;
            for (int i = 0; i < count - 1; i++)
            {
                if (levels[i] < levels[i + 1])
                    maxLevelIndex = i + 1;
                maxLevelIndex = i;
            }
            //将group加入深度最大的父节点中
            (parents[maxLevelIndex] as IElementGroup).Elements.Add(group);
            return group;
        }

        #region Left
        /// <summary>
        /// 左对齐
        /// </summary>
        /// <param name="group"></param>
        public static void ArrangeLeft(IElementGroup group, ILayoutHost host)
        {
            if (group == null)
                return;
            IElement[] eles = group.Elements.ToArray();
            ArrangeLeft(eles, host);
        }

        public static void ArrangeLeft(IElement[] eles, ILayoutHost host)
        {
            if (eles == null || eles.Length == 0)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            float newX;
            int count = eles.Length;
            //如果只选择了一个元素，则与图廓的左边对齐
            if (count == 1)
            {
                IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
                newX = boder.Location.X;
            }
            //选择了多个元素则以最左边的元素为准
            else
            {
                newX = (eles[0] as ISizableElement).Location.X;
                for (int i = 1; i < count; i++)
                {
                    if (!(eles[i] is ISizableElement))
                        return;
                    if (newX > (eles[i] as ISizableElement).Location.X)
                        newX = (eles[i] as ISizableElement).Location.X;

                }
            }
            for (int i = 0; i < count; i++)
                (eles[i] as ISizableElement).Location = new PointF(newX, (eles[i] as ISizableElement).Location.Y);
        }
        #endregion

        #region Right
        public static void ArrangeRight(IElementGroup group, ILayoutHost host)
        {
            if (group == null)
                return;
            IElement[] eles = group.Elements.ToArray();
            ArrangeRight(eles, host);
        }

        public static void ArrangeRight(IElement[] eles, ILayoutHost host)
        {
            if (eles == null || eles.Length == 0)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            int count = eles.Length;
            float newX;
            //如果只选择了一个元素，则与图廓的右边对齐
            if (count == 1)
            {
                IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
                newX = boder.Location.X + boder.Size.Width;
            }
            //选择了多个元素则以最右边的元素为准
            else
            {
                newX = (eles[0] as ISizableElement).Location.X + (eles[0] as ISizableElement).Size.Width;
                for (int i = 1; i < count; i++)
                {
                    if (!(eles[i] is ISizableElement))
                        return;
                    if (newX < (eles[i] as ISizableElement).Location.X + (eles[i] as ISizableElement).Size.Width)
                        newX = (eles[i] as ISizableElement).Location.X + (eles[i] as ISizableElement).Size.Width;
                }
            }
            for (int i = 0; i < count; i++)
                (eles[i] as ISizableElement).Location = new PointF(newX - (eles[i] as ISizableElement).Size.Width,
                                                                   (eles[i] as ISizableElement).Location.Y);
        }
        #endregion

        #region Top
        /// <summary>
        /// 顶端对齐
        /// </summary>
        /// <param name="group"></param>
        public static void ArrangeTop(IElementGroup group, ILayoutHost host)
        {
            if (group == null)
                return;
            IElement[] eles = group.Elements.ToArray();
            ArrangeTop(eles, host);
        }

        public static void ArrangeTop(IElement[] eles, ILayoutHost host)
        {
            if (eles == null || eles.Length == 0)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            float newY;// = (eles[0] as ISizableElement).Location.Y;
            int count = eles.Length;
            //如果只选择了一个元素，则与图廓的左边对齐
            if (count == 1)
            {
                IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
                newY = boder.Location.Y;
            }
            //选择了多个元素则以最左边的元素为准
            else
            {
                newY = (eles[0] as ISizableElement).Location.Y;
                for (int i = 1; i < count; i++)
                {
                    if (!(eles[i] is ISizableElement))
                        return;
                    if (newY > (eles[i] as ISizableElement).Location.Y)
                        newY = (eles[i] as ISizableElement).Location.Y;

                }
            }
            for (int i = 0; i < count; i++)
                (eles[i] as ISizableElement).Location = new PointF((eles[i] as ISizableElement).Location.X, newY);

        }
        #endregion

        /// <summary>
        /// 底端对齐
        /// </summary>
        /// <param name="eles"></param>
        /// <param name="host"></param>
        public static void ArrangeBottom(IElement[] eles, ILayoutHost host)
        {
            if (eles == null || eles.Length == 0)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            float newY;//= (eles[0] as ISizableElement).Location.Y + (eles[0] as ISizableElement).Size.Height;
            int count = eles.Length;
            //如果只选择了一个元素，则与图廓的右边对齐
            if (count == 1)
            {
                IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
                newY = boder.Location.Y + boder.Size.Height;
            }
            //选择了多个元素则以最右边的元素为准
            else
            {
                newY = (eles[0] as ISizableElement).Location.Y + (eles[0] as ISizableElement).Size.Height;
                for (int i = 1; i < count; i++)
                {
                    if (!(eles[i] is ISizableElement))
                        return;
                    if (newY < (eles[i] as ISizableElement).Location.Y + (eles[i] as ISizableElement).Size.Height)
                        newY = (eles[i] as ISizableElement).Location.Y + (eles[i] as ISizableElement).Size.Height;
                }
            }
            for (int i = 0; i < count; i++)
                (eles[i] as ISizableElement).Location = new PointF((eles[i] as ISizableElement).Location.X,
                                                                   newY - (eles[i] as ISizableElement).Size.Height);
        }


        public static void ArrangeLeftRightMid(IElement[] eles, ILayoutHost host)
        {
            if (eles == null || eles.Length == 0)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            int count = eles.Length;
            float newX;
            IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
            //如果只选择了一个元素，则与图廓的中线对齐
            if (count == 1)
                newX = boder.Location.X + boder.Size.Width / 2f;
            //选择了多个元素则以最中间的元素为准
            else
            {
                newX = (eles[0] as ISizableElement).Location.X + (eles[0] as ISizableElement).Size.Width / 2f;
                for (int i = 1; i < count; i++)
                {
                    if (!(eles[i] is ISizableElement))
                        return;
                    if (Math.Abs(newX - boder.Location.X + boder.Size.Width / 2f) >
                        Math.Abs((eles[i] as ISizableElement).Location.X + (eles[i] as ISizableElement).Size.Width / 2 - boder.Location.X - boder.Size.Width / 2f))
                        newX = (eles[i] as ISizableElement).Location.X + (eles[i] as ISizableElement).Size.Width / 2f;
                }
            }
            for (int i = 0; i < count; i++)
                (eles[i] as ISizableElement).Location = new PointF(newX - (eles[i] as ISizableElement).Size.Width/2f,
                                                                   (eles[i] as ISizableElement).Location.Y);

        }
    }
}
