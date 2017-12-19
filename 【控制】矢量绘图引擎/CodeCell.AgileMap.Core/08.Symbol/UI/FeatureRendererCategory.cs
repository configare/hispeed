using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public  class FeatureRendererCategory
    {
        public string Name = null;
        public List<FeatureRendererCategory> Children = null;

        public FeatureRendererCategory(string name)
        {
            Name = name;
        }

        public FeatureRendererCategory(string name, FeatureRendererCategory[] children)
            :this(name)
        {
            if (Children == null)
                Children = new List<FeatureRendererCategory>();
            Children.AddRange(children);
        }

        public void Add(FeatureRendererCategory child)
        {
            if (Children == null)
                Children = new List<FeatureRendererCategory>();
            Children.Add(child);
        }

        public virtual IFeaureRenderEditorControl GetFeaureRenderEditorControl()
        {
            return null;
        }
    }

    public class FeatureRendererCategorySimple:FeatureRendererCategory
    {
        public FeatureRendererCategorySimple(string name)
            : base(name)
        { 
        }

        public FeatureRendererCategorySimple(string name, FeatureRendererCategory[] children)
            : base(name, children) 
        {
        }

        public override IFeaureRenderEditorControl GetFeaureRenderEditorControl()
        {
            return new UCFeatureRendererSimple();
        }
    }

    public class FeatureRendererCategorySimpleTwoStep : FeatureRendererCategory
    {
        public FeatureRendererCategorySimpleTwoStep(string name)
            : base(name)
        {
        }

        public FeatureRendererCategorySimpleTwoStep(string name, FeatureRendererCategory[] children)
            : base(name, children)
        {
        }

        public override IFeaureRenderEditorControl GetFeaureRenderEditorControl()
        {
            return new UCFeatureRendererSimpleTwoStep();
        }
    }

    public class FeatureRendererCategoryComposite : FeatureRendererCategory
    {
        public FeatureRendererCategoryComposite(string name)
            : base(name)
        {
        }

        public FeatureRendererCategoryComposite(string name, FeatureRendererCategory[] children)
            : base(name, children)
        {
        }

        public override IFeaureRenderEditorControl GetFeaureRenderEditorControl()
        {
            return new UCFeatureRendererComposite();
        }
    }

    public class FeatureRendererCategoryUniqueValue : FeatureRendererCategory
    {
        public FeatureRendererCategoryUniqueValue(string name)
            : base(name)
        {
        }

        public FeatureRendererCategoryUniqueValue(string name, FeatureRendererCategory[] children)
            : base(name, children)
        {
        }

        public override IFeaureRenderEditorControl GetFeaureRenderEditorControl()
        {
            return new UCFeatureRendererUniqueValue();
        }
    }
}
