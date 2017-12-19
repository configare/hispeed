using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;

namespace Telerik.WinControls
{
    
    ///<exclude/> 
    
    public class ElementShapeEditor: UITypeEditor
    {
        private const int MaxLoaderExceptionsInMessageBox = 15;

        IWindowsFormsEditorService editorService = null;
		ArrayList shapes = null;
		bool indexChanged;

        public ElementShapeEditor()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
			return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
			shapes = new ArrayList();
			editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			ListBox listBox = CreateListBox(context, value);
			
			indexChanged = false;

			editorService.DropDownControl(listBox);

			if (!indexChanged)
			{
				return value;
			}

			if (listBox.SelectedIndex == 0)
			{
				return null;
			}

			if (listBox.SelectedIndex-1 > shapes.Count - 1)
			{
				bool newShape = true;

				if (context.Container == null && value is CustomShape)
				{
					newShape = false;
				}

				if (listBox.SelectedIndex-1 == shapes.Count && newShape)
				{
					CustomShape shape = CreateNewShape(context);
					return EditPoints(context, shape);
				}
				else
				{
					return EditPoints(context, (CustomShape)value);
				}
			}

			object result = shapes[listBox.SelectedIndex-1];

			if (result is Type)
			{
				if (context.Container != null)
				{
					foreach (IComponent component in context.Container.Components)
					{
						if (component.GetType() == (Type)result)
						{
							result = component;
							break;
						}
					}
					if (result is Type)
					{
						IDesignerHost host = (IDesignerHost)context.Container;
						result = host.CreateComponent((Type)result);
					}
				}
				else
				{
					result = Activator.CreateInstance((Type)result);
				}
			}

			shapes.Clear();
			
			return result;
        }

        private void listBox_SelectedValueChanged(object sender, EventArgs e)
        {
			indexChanged = true;
            if (editorService != null)
                editorService.CloseDropDown();
        }

		private ListBox CreateListBox(ITypeDescriptorContext context, object value)
		{
			ListBox listBox = new ListBox();

			listBox.SelectedValueChanged += new EventHandler(listBox_SelectedValueChanged);
			listBox.Dock = DockStyle.Fill;
			listBox.BorderStyle = BorderStyle.None;
			listBox.ItemHeight = 13;

			listBox.Items.Add("(none)");

			// Load standard shapes
			if (context.Container != null)
			{
				ITypeDiscoveryService discoveryService = (ITypeDiscoveryService)context.GetService(typeof(ITypeDiscoveryService));
				foreach (Type type in discoveryService.GetTypes(typeof(ElementShape), false))
				{
					if (type != typeof(CustomShape) && !type.IsAbstract && type.IsPublic)
					{
						listBox.Items.Add(type.Name);
						shapes.Add(type);

						if (value != null && value.GetType() == type)
						{
							listBox.SelectedIndex = listBox.Items.Count - 1;
						}
					}
				}
			}
			else
			{
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
                        if (!IsTelerikAssembly(assembly))
                            continue;
                        foreach (Type type in assembly.GetTypes())
						{
							if (type.IsClass && type.IsPublic && !type.IsAbstract &&
								typeof(ElementShape).IsAssignableFrom(type) &&
								type != typeof(CustomShape))
							{
								listBox.Items.Add(type.Name);
								shapes.Add(type);

								if (value != null && value.GetType() == type)
								{
									listBox.SelectedIndex = listBox.Items.Count - 1;
								}
							}
						}
					}
                    catch (ReflectionTypeLoadException e)
                    {
                        string message = e.Message + "\n\nLoader Exceptions:\n";
                        for (int i = 0; i < Math.Min(MaxLoaderExceptionsInMessageBox, e.LoaderExceptions.Length); i++)
                            message += e.LoaderExceptions[i].Message + "\n";
                        if (e.LoaderExceptions.Length > MaxLoaderExceptionsInMessageBox)
                            message += "More...";
                        MessageBox.Show(message, assembly.FullName);
                    }
					catch (Exception ex)
					{
                        MessageBox.Show(ex.Message, assembly.FullName);
					}
				}
			}
			
			// Load custom shape components
			if (context.Container != null)
			{
				foreach (IComponent component in context.Container.Components)
				{
					if (component is CustomShape)
					{
						listBox.Items.Add(component.Site.Name);
						shapes.Add(component);

						if (component == value)
						{
							listBox.SelectedIndex = listBox.Items.Count - 1;
						}
					}
				}

				listBox.Items.Add("Create new custom shape ...");
				if (value != null && value is CustomShape)
				{
					listBox.Items.Add("Edit points ...");
				}
			}
			else
			{
				if (value != null && value is CustomShape)
				{
					listBox.Items.Add("Edit points ...");
				}
				else
				{
					listBox.Items.Add("Create new custom shape ...");
				}
			}


			return listBox;
		}

        private static bool IsTelerikAssembly(Assembly asm)
        {
            if (asm == null)
                return false;

            if (asm.FullName.Contains("Telerik"))
                return true;

            AssemblyName[] names = asm.GetReferencedAssemblies();
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].FullName.Contains("Telerik"))
                    return true;
            }

            return false;
        }

		private CustomShape CreateNewShape(ITypeDescriptorContext context)
		{
			CustomShape shape = null;

			if (context.Container != null)
			{
				shape = (CustomShape)(context.Container as IDesignerHost).CreateComponent(typeof(CustomShape));
			}
			else
			{
				shape = new CustomShape();
			}

		    shape.CreateRectangleShape(20, 20, 200, 100);

            //shape.Dimension = new Rectangle(20, 20, 200, 100);

            //shape.Points.Add(new ShapePoint(20, 20));
            //shape.Points.Add(new ShapePoint(220, 20));
            //shape.Points.Add(new ShapePoint(220, 120));
            //shape.Points.Add(new ShapePoint(20, 120));

			return shape;
		}

		private CustomShape EditPoints(ITypeDescriptorContext context, CustomShape shape)
		{
			CustomShapeEditorForm editor = new CustomShapeEditorForm();

            //editor.EditorControl.Dimension = shape.Dimension;
			
            //foreach (ShapePoint point in shape.Points)
            //{
            //    editor.EditorControl.Points.Add(new ShapePoint(point));
            //}

		    shape = editor.EditShape(shape);

            //if (editor.ShowDialog() == DialogResult.OK)
            //{
                //IDesignerHost host = context.Container as IDesignerHost;
                //if (host != null)
                //{
                //    foreach (ShapePoint point in shape.Points)
                //    {
                //        host.DestroyComponent(point);
                //    }
                //}

                //shape.Points.Clear();

            //    foreach (ShapePoint point in editor.EditorControl.Points)
            //    {
            //        ShapePoint p = null;

            //        if (host != null)
            //        {
            //            p = (ShapePoint)host.CreateComponent(typeof(ShapePoint));
            //        }
            //        else
            //        {
            //            p = new ShapePoint();
            //        }

            //        p.X = point.X;
            //        p.Y = point.Y;
            //        p.ControlPoint1 = point.ControlPoint1;
            //        p.ControlPoint2 = point.ControlPoint2;
            //        p.Bezier = point.Bezier;
            //        p.Locked = point.Locked;
            //        shape.Points.Add(p);
            //    }
				
            //    shape.Dimension = editor.EditorControl.Dimension;
            //}

			return shape;
		}
    }
}
