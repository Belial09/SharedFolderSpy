using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Drawing.Design;
using System;



[DesignerAttribute(typeof(UserControl1Designer))]
public class UserControl1 : System.Windows.Forms.Control {
    
    //  This is an example Control Providing a collection with Different Objects in a collection
    private Buttons m_Buttons = new Buttons();
    
    // Required by the Windows Form Designer
    private System.ComponentModel.IContainer components;
    
    private ImageList m_ImageList;
    
    public UserControl1() {
        // This call is required by the Windows Form Designer.
        InitializeComponent();
        // Add any initialization after the InitializeComponent() call
        this.BackColor = Color.Red;
    }
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Editor(typeof(ButtonCollectionEditor), typeof(UITypeEditor))]
    public Buttons Buttons {
        get {
            return m_Buttons;
        }
    }
    
    public ImageList ImageList {
        get {
            return m_ImageList;
        }
        set {
            m_ImageList = value;
            mm_ImageList = value;
        }
    }
    
    event EventHandler SomeEvent;
    
    // UserControl1 overrides dispose to clean up the component list.
    protected override void Dispose(bool disposing) {
        if (disposing) {
            if (!(components == null)) {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }
    
    // NOTE: The following procedure is required by the Windows Form Designer
    // It can be modified using the Windows Form Designer.  
    // Do not modify it using the code editor.
    [System.Diagnostics.DebuggerStepThrough()]
    private void InitializeComponent() {
        components = new System.ComponentModel.Container();
    }
}
//  Base Class For Button Object
//  We Inherit from Component, DesignTimeVisible(False) Attribute prevents your objects created by your component or Control to appear in the component tray of the designer.
[DesignTimeVisible(false)]
public class ButtonBase : System.ComponentModel.Component {
    
    // Memory Variables For Properties
    private ButtonTypes m_ButtonType;
    
    private int m_ImageIndex = -1;
    
    private bool m_Value;
    
    private Buttons m_Collection;
    
    private int m_Width;
    
    public ButtonBase() {
    }
    
    // Overloaded Constructor
    public ButtonBase(ButtonTypes Type, int Width) {
        m_ButtonType = Type;
        m_Width = Width;
    }
    
    // This property is used to give a reference to the collection in which the object is.
    // By means of this object variable we can raise an event through the collection when one of the properties is changed
    public Buttons Collection {
        set {
            m_Collection = value;
        }
    }
    
    // We give a browsable(False) attribute to Button type, because this property will be set in inherited objects Constructor( Sub New )
    [Browsable(false)]
    public ButtonTypes ButtonType {
        get {
            return m_ButtonType;
        }
        set {
            m_ButtonType = value;
        }
    }
    
    [DefaultValue(-1)]
    [TypeConverter(typeof(EImageIndexConverter))]
    [Editor(typeof(EImageIndexEditor), typeof(UITypeEditor))]
    public int ImageIndex {
        get {
            return m_ImageIndex;
        }
        set {
            m_ImageIndex = value;
            PropertyChanged();
        }
    }
    
    public bool Value {
        get {
            return m_Value;
        }
        set {
            m_Value = value;
        }
    }
    
    public int Width {
        get {
            return m_Width;
        }
    }
    
    private void PropertyChanged() {
        // Check if the collection is a valid object if not during design time you and up with a message 'Object is not set to an Instance' But Your Program Works
        // By the way I am looking forward for the IsNot operator in VS 2005, because every time I forget the Not Operator and need to navigate back
        if (!(m_Collection == null)) {
            m_Collection.RaisePropertyChangedEvent();
        }
    }
    
    //  We need the following Enum to differentiate between different objects
    public enum ButtonTypes {
        
        PushButton = 0,
        
        GroupButton = 1,
        
        ToggleButton = 2,
        
        PlaceHolder = 3,
        
        Seperator = 4,
    }
}
// We Make PushButton Serializable and assign a type converter to control the sserialization of the object. For Code Clearity it is advisable to have the Converter as a nested class 
[Serializable()]
[TypeConverter(typeof(PushButton.PushButtonConverter))]
public class PushButton : ButtonBase {
    
    // Constructors
    public PushButton() {
        this.ButtonType = ButtonBase.ButtonTypes.PushButton;
    }
    
    // We Dont Want a Push Button to Expose a Value Property so we shadow it with browsable(False) atributr
    [Browsable(false)]
    new object Value {
        get {
            // No Code is Required for Get and Set
        }
        set {
            
        }
    }
    
    [Browsable(false)]
    new object Width {
        get {
            
        }
        set {
            
        }
    }
    
    event EventHandler Click;
    
    internal void OnClick() {
        Click(this, new EventArgs());
    }
    
    // Now Exciting Part of Code Serialization
    class PushButtonConverter : TypeConverter {
        
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
            // What we are saying to the serializor, if the seriazor asks for an InstanceDescriptor, we can handle it
            if ((destinationType == typeof(InstanceDescriptor))) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
            if ((destinationType == typeof(InstanceDescriptor))) {
                // PushButton object does not have a constructor with parameters so we just return the Sub New Constructor
                // First paramater return the Constructor, Second must be Nothing because Constructor does not have any parameters, and Third parameter basically tell the serializor definition is not complete and properties will be defined afterwards. This is required because we want to see the generated Code as follows
                //  Friend WithEvents PushButton1 as PushButton
                //  In InitializeComponent
                //  Me.PushButton1 = new PushButton
                //  ...   .AddRange(new Object(),{me.PushButton1, ....  other Buttons .... })
                //  Me.PushBotton1.ImageIndex = 0
                //  Other Properties follows
                return new InstanceDescriptor(typeof(PushButton).GetConstructor(new Type[0]), null, false);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
[Serializable()]
[TypeConverter(typeof(ToggleButton.ToggleButtonConverter))]
public class ToggleButton : ButtonBase {
    
    public ToggleButton() {
        this.ButtonType = ButtonBase.ButtonTypes.ToggleButton;
    }
    
    [Browsable(false)]
    new object Width {
        get {
            
        }
        set {
            
        }
    }
    
    event EventHandler ValueChanged;
    
    internal void OnValueChanged() {
        ValueChanged(this, new EventArgs());
    }
    
    class ToggleButtonConverter : TypeConverter {
        
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
            // What we are saying to the serializor, if the seriazor asks for an InstanceDescriptor, we can handle it
            if ((destinationType == typeof(InstanceDescriptor))) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
            if ((destinationType == typeof(InstanceDescriptor))) {
                return new InstanceDescriptor(typeof(ToggleButton).GetConstructor(new Type[0]), null, false);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
[Serializable()]
[TypeConverter(typeof(ButtonSeperator.ButtonSeperatorConverter))]
public class ButtonSeperator {
    
    private string m_Text;
    
    public ButtonSeperator() {
        m_Text = "Seperator";
    }
    
    public string Text {
        get {
            return m_Text;
        }
    }
    
    class ButtonSeperatorConverter : TypeConverter {
        
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
            if ((destinationType == typeof(InstanceDescriptor))) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        
        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
            if ((destinationType == typeof(InstanceDescriptor))) {
                return new InstanceDescriptor(typeof(ButtonSeperator).GetConstructor(new Type[0]), null, true);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
[Serializable()]
[TypeConverter(typeof(PlaceHolder.PlaceHolderConverter))]
public class PlaceHolder {
    
    private int m_Width;
    
    public PlaceHolder() {
    }
    
    public PlaceHolder(int Width) {
        m_Width = Width;
    }
    
    public int Width {
        get {
            return m_Width;
        }
        set {
            m_Width = value;
        }
    }
    
    class PlaceHolderConverter : TypeConverter {
        
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
            if ((destType == typeof(InstanceDescriptor))) {
                return true;
            }
            return base.CanConvertTo(context, destType);
        }
        
        public override void ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
            if ((destType == typeof(InstanceDescriptor))) {
                PlaceHolder MyObject = ((PlaceHolder)(value));
                return new InstanceDescriptor(typeof(PlaceHolder).GetConstructor(new Type[] {
                                typeof(int)}), new object[] {
                            MyObject.Width}, true);
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
//  A Strong typed collection for different type of buttons
//  Serializable attribute is required to generate code for your objects defined during design time 
[Serializable()]
public class Buttons : CollectionBase {
    
    public ButtonBase this[int Index] {
        get {
            if ((list(Index).GetType() == PushButton)) {
                return ((ButtonBase)(list(Index)));
            }
            if ((list(Index).GetType() == ToggleButton)) {
                return ((ButtonBase)(list(Index)));
            }
            if ((list(Index).GetType() == PlaceHolder)) {
                return new ButtonBase(ButtonBase.ButtonTypes.PlaceHolder, ((PlaceHolder)(List(Index))).Width);
            }
            if ((List(Index).GetType() == ButtonSeperator)) {
                return new ButtonBase(ButtonBase.ButtonTypes.Seperator, 0);
            }
        }
    }
    
    // Event To Notify Parent when a property is changed during Design or Run Time so the control can Paint itself
    event EventHandler PropertyChaged;
    
    public object Add(object Item) {
        if ((!(Item.GetType() == ButtonSeperator) 
                    && !(Item.GetType() == PlaceHolder))) {
            ((ButtonBase)(Item)).Collection = this;
        }
        list.Add(Item);
        return Item;
    }
    
    public void AddRange(object[] Items) {
        foreach (object Item in Items) {
            if ((!(Item.GetType() == ButtonSeperator) 
                        && !(Item.GetType() == PlaceHolder))) {
                ((ButtonBase)(Item)).Collection = this;
            }
            list.Add(Item);
        }
    }
    
    internal void RaisePropertyChangedEvent() {
        PropertyChaged();
    }
}

class ButtonCollectionEditor : System.ComponentModel.Design.CollectionEditor {
    
    private System.Type[] Types;
    
    ButtonCollectionEditor(System.Type type) : 
            base(type) {
        Types = new System.Type[] {
                typeof(PushButton),
                typeof(ButtonSeperator),
                typeof(PlaceHolder),
                typeof(ToggleButton)};
    }
    
    protected override System.Type[] CreateNewItemTypes() {
        return Types;
    }
}
class EImageIndexConverter : ImageIndexConverter {
    
    public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
        if ((value.GetType() == String)) {
            if (((value != "(none)") 
                        && (value != null))) {
                try {
                    return int.Parse(value);
                }
                catch (Exception ex) {
                    return -1;
                }
            }
            else {
                return -1;
            }
        }
        else {
            return null;
        }
    }
    
    public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
        if ((value.GetType() == Integer)) {
            if ((value != -1)) {
                return value.ToString();
            }
            else {
                return "(none)";
            }
        }
        else {
            return -1;
        }
    }
    
    public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context) {
        ArrayList Ar = new ArrayList();
        Ar.Add(-1);
        ImageList m_imagel;
        m_imagel = mm_ImageList;
        if ((mm_ImageList == null)) {
            m_imagel = null;
        }
        else {
            m_imagel = mm_ImageList;
        }
        if (!(m_imagel == null)) {
            for (int i = 0; (i 
                        <= (m_imagel.Images.Count - 1)); i++) {
                Ar.Add(i);
            }
        }
        return new StandardValuesCollection(Ar);
    }
    
    public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context) {
        if ((context.Instance == null)) {
            return false;
        }
        else {
            return true;
        }
    }
}
class EImageIndexEditor : UITypeEditor {
    
    public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context) {
        return true;
    }
    
    public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e) {
        int m_imageIdx;
        m_imageIdx = int.Parse(e.Value);
        ImageList m_imagel;
        if ((mm_ImageList == null)) {
            m_imagel = null;
        }
        else {
            m_imagel = mm_ImageList;
        }
        if (!(m_imagel == null)) {
            if (((m_imageIdx >= 0) 
                        && (m_imageIdx < m_imagel.Images.Count))) {
                e.Graphics.DrawImage(m_imagel.Images[int.Parse(e.Value)], e.Bounds);
            }
        }
    }
}
public class UserControl1Designer : System.Windows.Forms.Design.ControlDesigner {
    
    public override void Initialize(System.ComponentModel.IComponent component) {
        base.Initialize(component);
        ISelectionService ss = ((ISelectionService)(GetService(typeof(ISelectionService))));
        if (!(ss == null)) {
            ss.SelectionChanged += new System.EventHandler(this.OnSelectionChanged);
        }
    }
    
    private void OnSelectionChanged(object sender, EventArgs e) {
        ISelectionService ss = ((ISelectionService)(sender));
        if (!(ss == null)) {
            if ((ss.PrimarySelection.GetType() == UserControl1)) {
                mm_ImageList = ((UserControl1)(ss.PrimarySelection)).ImageList;
            }
        }
    }
}

