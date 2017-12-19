using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout.GDIPlus
{
    internal static class ElementsEditor
    {
        /// <summary>
        /// 将元素编组
        /// </summary>
        /// <param name="elements">需要编组的元素</param>
        /// <param name="host">当前的layoutHost</param>
        /// <returns></returns>
        public static IElementGroup ElementsToGroup(IElement[] elements, ILayoutHost host)
        {
            if (host == null)
                return null;
            if (elements == null || elements.Length == 0)
                return null;
            if (elements.Length < 2)
                return null;
            ISizableElementGroup group = new SizableElementGroup(elements);
            List<IElement> eles = group.Elements;
            int count = eles.Count;
            if (count < 2)
                return null;
            IElement[] parents = new IElement[count];
            IElement parent;
            //将所选element从其父节点中移除
            for (int i = 0; i < count; i++)
            {
                parent = host.LayoutRuntime.Layout.FindParent(eles[i]);
                if (parent == null)
                    return null;
                parents[i] = parent;
                (parents[i] as IElementGroup).Elements.Remove(eles[i]);
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
            //其他的父节点如果是组合，其中如果没有元素或者只有一个组合，则将其删除
            for (int i = 0; i < parents.Length; i++)
            {
                if (i == maxLevelIndex)
                    continue;
                if(!(parents[i] is ISizableElementGroup))
                    continue;
                IElement grandPa = host.LayoutRuntime.Layout.FindParent(parents[i]);
                List<IElement> pEles = (parents[i] as ISizableElementGroup).Elements;
                if (pEles == null || pEles.Count == 0)
                    (grandPa as IElementGroup).Elements.Remove(parents[i]);
                if (pEles.Count == 1)
                {
                    (grandPa as IElementGroup).Elements.Add(pEles.ToArray()[0]);
                    (grandPa as IElementGroup).Elements.Remove(parents[i]);
                }
            }
            return group;
        }

        /// <summary>
        /// 取消元素组合
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="host"></param>
        public static void ElementsUnGroup(IElement[] elements, ILayoutHost host)
        {
            if (host == null)
                return;
            if (elements == null || elements.Length == 0)
                return;
            if (elements.Length != 1)
                return;
            IElement ele = elements[0];
            if (ele is ISizableElementGroup) //选中的是元素的组合时
                UnGroupSizableGroup(ele as ISizableElementGroup, host);
            else  //选中的是组合中的单个元素时
                UnGroupSizableElement(ele, host);

        }

        private static void UnGroupSizableGroup(ISizableElementGroup group, ILayoutHost host)
        {
            if (group == null)
                return;
            List<IElement> eles = group.Elements;
            IElementGroup parent = host.LayoutRuntime.Layout.FindParent(group) as IElementGroup;
            if (parent == null)
                return;
            foreach (IElement e in eles)
                parent.Elements.Add(e);
            parent.Elements.Remove(group);
        }

        private static void UnGroupSizableElement(IElement ele, ILayoutHost host)
        {
            if (ele == null)
                return;
            ISizableElementGroup group = host.LayoutRuntime.Layout.FindParent(ele) as ISizableElementGroup;
            if (group == null)
                return;  //选中的元素没有被组合
            UnGroupSizableGroup(group, host);
        }

        /// <summary>
        /// 左对齐
        /// </summary>
        /// <param name="group"></param>
        public static void AligmentLeft(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
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
                (eles[i] as ISizableElement).ApplyLocation(newX - (eles[i] as ISizableElement).Location.X, 0);
        }

        /// <summary>
        /// 右对齐
        /// </summary>
        /// <param name="eles"></param>
        /// <param name="host"></param>
        public static void AligmentRight(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
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
                (eles[i] as ISizableElement).ApplyLocation(newX - (eles[i] as ISizableElement).Size.Width
                                                           - (eles[i] as ISizableElement).Location.X, 0);
        }

        /// <summary>
        /// 顶端对齐
        /// </summary>
        /// <param name="group"></param>
        public static void AligmentTop(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
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
                (eles[i] as ISizableElement).ApplyLocation(0, newY - (eles[i] as ISizableElement).Location.Y);
        }

        /// <summary>
        /// 底端对齐
        /// </summary>
        /// <param name="eles"></param>
        /// <param name="host"></param>
        public static void AligmentBottom(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
            if (eles == null || eles.Length == 0)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            float newY;
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
                (eles[i] as ISizableElement).ApplyLocation(0, newY - (eles[i] as ISizableElement).Size.Height
                                                              - (eles[i] as ISizableElement).Location.Y);
        }

        /// <summary>
        /// 左右居中
        /// </summary>
        /// <param name="eles"></param>
        /// <param name="host"></param>
        public static void AligmentLeftRightMid(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
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
                    if (Math.Abs(newX - boder.Location.X - boder.Size.Width / 2f) >
                        Math.Abs((eles[i] as ISizableElement).Location.X + (eles[i] as ISizableElement).Size.Width / 2f - boder.Location.X - boder.Size.Width / 2f))
                        newX = (eles[i] as ISizableElement).Location.X + (eles[i] as ISizableElement).Size.Width / 2f;
                }
            }
            for (int i = 0; i < count; i++)
                (eles[i] as ISizableElement).ApplyLocation(newX - (eles[i] as ISizableElement).Size.Width / 2f
                                                           - (eles[i] as ISizableElement).Location.X, 0);
        }

        /// <summary>
        /// 上下居中
        /// </summary>
        /// <param name="eles"></param>
        /// <param name="host"></param>
        public static void AligmentTopBottomMid(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
            if (eles == null || eles.Length == 0)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            int count = eles.Length;
            float newY;
            IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
            //如果只选择了一个元素，则与图廓的中线对齐
            if (count == 1)
                newY = boder.Location.Y + boder.Size.Height / 2f;
            //选择了多个元素则以最中间的元素为准
            else
            {
                newY = (eles[0] as ISizableElement).Location.Y + (eles[0] as ISizableElement).Size.Height / 2f;
                for (int i = 1; i < count; i++)
                {
                    if (!(eles[i] is ISizableElement))
                        return;
                    if (Math.Abs(newY - boder.Location.Y - boder.Size.Height / 2f) >
                        Math.Abs((eles[i] as ISizableElement).Location.Y + (eles[i] as ISizableElement).Size.Height / 2f - boder.Location.Y - boder.Size.Height / 2f))
                        newY = (eles[i] as ISizableElement).Location.Y + (eles[i] as ISizableElement).Size.Height / 2f;
                }
            }
            for (int i = 0; i < count; i++)
                (eles[i] as ISizableElement).ApplyLocation(0, newY - (eles[i] as ISizableElement).Size.Height / 2f
                                                           - (eles[i] as ISizableElement).Location.Y);
        }

        /// <summary>
        /// 纵向分布,效果相当于只选择了一个元素的上下居中分布
        /// </summary>
        /// <param name="eles"></param>
        /// <param name="host"></param>
        public static void AligmentVertical(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
            if (eles == null || eles.Length != 1)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            float newY;
            IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
            newY = boder.Location.Y + boder.Size.Height / 2f;
            (eles[0] as ISizableElement).ApplyLocation(0, newY - (eles[0] as ISizableElement).Size.Height / 2f
                                                       - (eles[0] as ISizableElement).Location.Y);
        }

        /// <summary>
        /// 横向分布，效果相当于只选择了一个元素的左右居中分布
        /// </summary>
        /// <param name="eles"></param>
        /// <param name="host"></param>
        public static void AligmentHorizontal(IElement[] eles, ILayoutHost host)
        {
            if (host == null)
                return;
            if (eles == null || eles.Length != 1)
                return;
            if (!(eles[0] is ISizableElement))
                return;
            float newX;
            IBorder boder = (host.LayoutRuntime.Layout as Layout).GetBorder();
            newX = boder.Location.X + boder.Size.Width / 2f;
            (eles[0] as ISizableElement).ApplyLocation(newX - (eles[0] as ISizableElement).Size.Width / 2f
                                                       - (eles[0] as ISizableElement).Location.X, 0);
        }

        /// <summary>
        /// 横向填充
        /// </summary>
        /// <param name="host"></param>
        public static void HorizontalStrech(ILayoutHost host)
        {
            if (host == null)
                return;
            if (host.LayoutRuntime == null)
                return;
            ILayout layout = host.LayoutRuntime.Layout;
            if (layout == null)
                return;
            List<IElement> elements = layout.Elements;
            if (elements == null || elements.Count == 0)
                return;
            foreach (IElement ele in elements)
            {
                if (ele is IDataFrame)
                {
                    (ele as IDataFrame).ApplyLocation(0 - (ele as IDataFrame).Location.X, 0);
                    (ele as IDataFrame).ApplySize(layout.Size.Width - (ele as IDataFrame).Size.Width, 0);
                }
            }
        }

        /// <summary>
        /// 纵向填充
        /// </summary>
        /// <param name="host"></param>
        public static void VerticalStrech(ILayoutHost host)
        {
            if (host == null)
                return;
            if (host.LayoutRuntime == null)
                return;
            ILayout layout = host.LayoutRuntime.Layout;
            if (layout == null)
                return;
            List<IElement> elements = layout.Elements;
            if (elements == null || elements.Count == 0)
                return;
            foreach (IElement ele in elements)
            {
                if (ele is IDataFrame)
                {
                    (ele as IDataFrame).ApplyLocation(0, 0 - (ele as IDataFrame).Location.Y);
                    (ele as IDataFrame).ApplySize(0, layout.Size.Height - (ele as IDataFrame).Size.Height);
                }
            }
        }
    }
}
