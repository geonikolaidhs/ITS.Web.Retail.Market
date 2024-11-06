#if __RR
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace ITS.POS.Tools.FormDesigner.Loader
{
    //class CustomCodeDomHostLoader
      [SecurityCritical]
  [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
  public abstract class CustomCodeDomHostLoader : BasicDesignerLoader, INameCreationService, IDesignerSerializationService
  {
    private static TraceSwitch traceCDLoader;
    private static CodeMarkers codemarkers;
    private static readonly int StateCodeDomDirty;
    private static readonly int StateCodeParserChecked;
    private static readonly int StateOwnTypeResolution;
    private BitVector32 _state;
    private IExtenderProvider[] _extenderProviders;
    private IExtenderProviderService _extenderProviderService;
    private ICodeGenerator _codeGenerator;
    private CodeDomSerializer _rootSerializer;
    private TypeCodeDomSerializer _typeSerializer;
    private CodeCompileUnit _documentCompileUnit;
    private CodeNamespace _documentNamespace;
    private CodeTypeDeclaration _documentType;

    protected abstract CodeDomProvider CodeDomProvider { get; }

    protected abstract ITypeResolutionService TypeResolutionService { get; }

    static CustomCodeDomHostLoader()
    {
      CustomCodeDomHostLoader.traceCDLoader = new TraceSwitch("CustomCodeDomHostLoader", "Trace CustomCodeDomHostLoader");
      CustomCodeDomHostLoader.codemarkers = CodeMarkers.Instance;
      CustomCodeDomHostLoader.StateCodeDomDirty = BitVector32.CreateMask();
      CustomCodeDomHostLoader.StateCodeParserChecked = BitVector32.CreateMask(CustomCodeDomHostLoader.StateCodeDomDirty);
      CustomCodeDomHostLoader.StateOwnTypeResolution = BitVector32.CreateMask(CustomCodeDomHostLoader.StateCodeParserChecked);
    }

    protected CustomCodeDomHostLoader():base()
    {
      this._state = new BitVector32();
      
    }

    public override void Dispose()
    {
      IDesignerHost designerHost = this.GetService(typeof (IDesignerHost)) as IDesignerHost;
      IComponentChangeService componentChangeService = this.GetService(typeof (IComponentChangeService)) as IComponentChangeService;
      if (componentChangeService != null)
      {
        // ISSUE: method pointer
          componentChangeService.ComponentRemoved -= OnComponentRemoved;// new ComponentEventHandler((object)this, __methodptr(OnComponentRemoved));
        // ISSUE: method pointer
          componentChangeService.ComponentRename -= OnComponentRename;// new ComponentRenameEventHandler((object)this, __methodptr(OnComponentRename));
      }
      if (designerHost != null)
      {
        designerHost.RemoveService(typeof (INameCreationService));
        designerHost.RemoveService(typeof (IDesignerSerializationService));
        designerHost.RemoveService(typeof (ComponentSerializationService));
        if (this._state[CustomCodeDomHostLoader.StateOwnTypeResolution])
        {
          designerHost.RemoveService(typeof (ITypeResolutionService));
          this._state[CustomCodeDomHostLoader.StateOwnTypeResolution] = false;
        }
      }
      if (this._extenderProviderService != null)
      {
        foreach (IExtenderProvider provider in this._extenderProviders)
          this._extenderProviderService.RemoveExtenderProvider(provider);
      }
      base.Dispose();
    }

    protected override void Initialize()
    {
      base.Initialize();
      // ISSUE: method pointer
      this.LoaderHost.AddService(typeof(ComponentSerializationService), OnCreateService);
          //new ServiceCreatorCallback((object) this, __methodptr(OnCreateService)));
      this.LoaderHost.AddService(typeof (INameCreationService), (object) this);
      this.LoaderHost.AddService(typeof (IDesignerSerializationService), (object) this);
      if (this.GetService(typeof (ITypeResolutionService)) == null)
      {
        ITypeResolutionService resolutionService = this.TypeResolutionService;
        if (resolutionService == null)
          throw new InvalidOperationException(System.Design.SR.GetString("CustomCodeDomHostLoaderNoTypeResolution"));
        this.LoaderHost.AddService(typeof (ITypeResolutionService), (object) resolutionService);
        this._state[CustomCodeDomHostLoader.StateOwnTypeResolution] = true;
      }
      this._extenderProviderService = this.GetService(typeof (IExtenderProviderService)) as IExtenderProviderService;
      if (this._extenderProviderService == null)
        return;
      this._extenderProviders = new IExtenderProvider[2]
      {
        (IExtenderProvider) new CustomCodeDomHostLoader.ModifiersExtenderProvider(),
        (IExtenderProvider) new CustomCodeDomHostLoader.ModifiersInheritedExtenderProvider()
      };
      foreach (IExtenderProvider provider in this._extenderProviders)
        this._extenderProviderService.AddExtenderProvider(provider);
    }

    protected override bool IsReloadNeeded()
    {
      if (!base.IsReloadNeeded())
        return false;
      if (this._documentType == null)
        return true;
      ICodeDomDesignerReload domDesignerReload = this.CodeDomProvider as ICodeDomDesignerReload;
      if (domDesignerReload == null)
        return true;
      bool flag = true;
      string name = this._documentType.Name;
      try
      {
        this.ClearDocument();
        this.EnsureDocument(this.GetService(typeof (IDesignerSerializationManager)) as IDesignerSerializationManager);
      }
      catch
      {
      }
      if (this._documentCompileUnit != null)
        flag = ((domDesignerReload.ShouldReloadDesigner(this._documentCompileUnit) ? 1 : 0) | (this._documentType == null ? 1 : (!this._documentType.Name.Equals(name) ? 1 : 0))) != 0;
      return flag;
    }

    protected override void OnBeginLoad()
    {
      IComponentChangeService componentChangeService = (IComponentChangeService) this.GetService(typeof (IComponentChangeService));
      if (componentChangeService != null)
      {
        // ISSUE: method pointer
          componentChangeService.ComponentRemoved -= OnComponentRemoved;
            //new ComponentEventHandler( this, OnComponentRemoved);
        // ISSUE: method pointer
          componentChangeService.ComponentRename -= OnComponentRename;
            //new ComponentRenameEventHandler((object) this, OnComponentRename);
      }
      base.OnBeginLoad();
    }

    protected override void OnBeginUnload()
    {
      base.OnBeginUnload();
      this.ClearDocument();
    }

    protected override void OnEndLoad(bool successful, ICollection errors)
    {
      base.OnEndLoad(successful, errors);
      if (!successful)
        return;
      IComponentChangeService componentChangeService = (IComponentChangeService) this.GetService(typeof (IComponentChangeService));
      if (componentChangeService == null)
        return;
      // ISSUE: method pointer
      componentChangeService.ComponentRemoved += OnComponentRemoved;// new ComponentEventHandler((object)this, __methodptr(OnComponentRemoved));
      // ISSUE: method pointer
      componentChangeService.ComponentRename += OnComponentRename;// new ComponentRenameEventHandler((object)this, __methodptr(OnComponentRename));
    }

    protected abstract CodeCompileUnit Parse();

    protected override void PerformFlush(IDesignerSerializationManager manager)
    {
      CodeTypeDeclaration newDecl = (CodeTypeDeclaration) null;
      if (this._rootSerializer != null)
        newDecl = this._rootSerializer.Serialize(manager, (object) this.LoaderHost.RootComponent) as CodeTypeDeclaration;
      else if (this._typeSerializer != null)
        newDecl = this._typeSerializer.Serialize(manager, (object) this.LoaderHost.RootComponent, (ICollection) this.LoaderHost.Container.Components);
      CustomCodeDomHostLoader.codemarkers.CodeMarker(7513);
      if (newDecl == null || !this.IntegrateSerializedTree(manager, newDecl))
        return;
      CustomCodeDomHostLoader.codemarkers.CodeMarker(7515);
      this.Write(this._documentCompileUnit);
    }

    protected override void PerformLoad(IDesignerSerializationManager manager)
    {
      this.EnsureDocument(manager);
      CustomCodeDomHostLoader.codemarkers.CodeMarker(7527);
      if (this._rootSerializer != null)
        this._rootSerializer.Deserialize(manager, (object) this._documentType);
      else
        this._typeSerializer.Deserialize(manager, this._documentType);
      CustomCodeDomHostLoader.codemarkers.CodeMarker(7528);
      this.SetBaseComponentClassName(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}.{1}", new object[2]
      {
        (object) this._documentNamespace.Name,
        (object) this._documentType.Name
      }));
    }

    protected virtual void OnComponentRename(object component, string oldName, string newName)
    {
      if (this.LoaderHost.RootComponent == component)
      {
        if (this._documentType == null)
          return;
        this._documentType.Name = newName;
      }
      else
      {
        if (this._documentType == null)
          return;
        CodeTypeMemberCollection members = this._documentType.Members;
        for (int index = 0; index < members.Count; ++index)
        {
          if (members[index] is CodeMemberField && members[index].Name.Equals(oldName) && ((CodeMemberField) members[index]).Type.BaseType.Equals(TypeDescriptor.GetClassName(component)))
          {
            members[index].Name = newName;
            break;
          }
        }
      }
    }

    protected abstract void Write(CodeCompileUnit unit);

    ICollection IDesignerSerializationService.Deserialize(object serializationData)
    {
      if (!(serializationData is SerializationStore))
      {
        Exception exception = (Exception) new ArgumentException(System.Design.SR.GetString("CustomCodeDomHostLoaderBadSerializationObject"));
        exception.HelpLink = "CustomCodeDomHostLoaderBadSerializationObject";
        throw exception;
      }
      else
      {
        ComponentSerializationService serializationService = this.GetService(typeof (ComponentSerializationService)) as ComponentSerializationService;
        if (serializationService == null)
          this.ThrowMissingService(typeof (ComponentSerializationService));
        return serializationService.Deserialize((SerializationStore) serializationData, this.LoaderHost.Container);
      }
    }

    object IDesignerSerializationService.Serialize(ICollection objects)
    {
      if (objects == null)
        objects = (ICollection) new object[0];
      ComponentSerializationService serializationService = this.GetService(typeof (ComponentSerializationService)) as ComponentSerializationService;
      if (serializationService == null)
        this.ThrowMissingService(typeof (ComponentSerializationService));
      SerializationStore store = serializationService.CreateStore();
      using (store)
      {
        foreach (object obj in (IEnumerable) objects)
          serializationService.Serialize(store, obj);
      }
      return (object) store;
    }

    string INameCreationService.CreateName(IContainer container, Type dataType)
    {
      if (dataType == (Type) null)
        throw new ArgumentNullException("dataType");
      string name = dataType.Name;
      StringBuilder stringBuilder = new StringBuilder(name.Length);
      for (int startIndex = 0; startIndex < name.Length; ++startIndex)
      {
        if (char.IsUpper(name[startIndex]) && (startIndex == 0 || startIndex == name.Length - 1 || char.IsUpper(name[startIndex + 1])))
        {
          stringBuilder.Append(char.ToLower(name[startIndex], CultureInfo.CurrentCulture));
        }
        else
        {
          stringBuilder.Append(name.Substring(startIndex));
          break;
        }
      }
      stringBuilder.Replace('`', '_');
      string str = ((object) stringBuilder).ToString();
      CodeTypeDeclaration codeTypeDeclaration = this._documentType;
      Hashtable hashtable = new Hashtable((IEqualityComparer) StringComparer.CurrentCultureIgnoreCase);
      if (codeTypeDeclaration != null)
      {
        foreach (CodeTypeMember codeTypeMember in (CollectionBase) codeTypeDeclaration.Members)
          hashtable[(object) codeTypeMember.Name] = (object) codeTypeMember;
      }
      string index;
      if (container != null)
      {
        int num = 0;
        bool flag;
        do
        {
          ++num;
          flag = false;
          index = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}", new object[2]
          {
            (object) str,
            (object) num.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          });
          if (container != null && container.Components[index] != null)
            flag = true;
          if (!flag && hashtable[(object) index] != null)
            flag = true;
        }
        while (flag);
      }
      else
        index = str;
      if (this._codeGenerator == null)
      {
        CodeDomProvider codeDomProvider = this.CodeDomProvider;
        if (codeDomProvider != null)
          this._codeGenerator = codeDomProvider.CreateGenerator();
      }
      if (this._codeGenerator != null)
        index = this._codeGenerator.CreateValidIdentifier(index);
      return index;
    }

    bool INameCreationService.IsValidName(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        return false;
      if (this._codeGenerator == null)
      {
        CodeDomProvider codeDomProvider = this.CodeDomProvider;
        if (codeDomProvider != null)
          this._codeGenerator = codeDomProvider.CreateGenerator();
      }
      if (this._codeGenerator != null && (!this._codeGenerator.IsValidIdentifier(name) || !this._codeGenerator.IsValidIdentifier(name + "Handler")))
        return false;
      if (!this.Loading)
      {
        CodeTypeDeclaration codeTypeDeclaration = this._documentType;
        if (codeTypeDeclaration != null)
        {
          foreach (CodeTypeMember codeTypeMember in (CollectionBase) codeTypeDeclaration.Members)
          {
            if (string.Equals(codeTypeMember.Name, name, StringComparison.OrdinalIgnoreCase))
              return false;
          }
        }
        if (this.Modified && this.LoaderHost.Container.Components[name] != null)
          return false;
      }
      return true;
    }

    void INameCreationService.ValidateName(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
      {
        Exception exception = (Exception) new ArgumentException(System.Design.SR.GetString("CustomCodeDomHostLoaderInvalidBlankIdentifier"));
        exception.HelpLink = "CustomCodeDomHostLoaderInvalidIdentifier";
        throw exception;
      }
      else
      {
        if (this._codeGenerator == null)
        {
          CodeDomProvider codeDomProvider = this.CodeDomProvider;
          if (codeDomProvider != null)
            this._codeGenerator = codeDomProvider.CreateGenerator();
        }
        if (this._codeGenerator != null)
        {
          this._codeGenerator.ValidateIdentifier(name);
          try
          {
            this._codeGenerator.ValidateIdentifier(name + "_");
          }
          catch
          {
            Exception exception = (Exception) new ArgumentException(System.Design.SR.GetString("CustomCodeDomHostLoaderInvalidIdentifier", new object[1]
            {
              (object) name
            }));
            exception.HelpLink = "CustomCodeDomHostLoaderInvalidIdentifier";
            throw exception;
          }
        }
        if (this.Loading)
          return;
        bool flag = false;
        CodeTypeDeclaration codeTypeDeclaration = this._documentType;
        if (codeTypeDeclaration != null)
        {
          foreach (CodeTypeMember codeTypeMember in (CollectionBase) codeTypeDeclaration.Members)
          {
            if (string.Equals(codeTypeMember.Name, name, StringComparison.OrdinalIgnoreCase))
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag && this.Modified && this.LoaderHost.Container.Components[name] != null)
          flag = true;
        if (!flag)
          return;
        Exception exception1 = (Exception) new ArgumentException(System.Design.SR.GetString("CustomCodeDomHostLoaderDupComponentName", new object[1]
        {
          (object) name
        }));
        exception1.HelpLink = "CustomCodeDomHostLoaderDupComponentName";
        throw exception1;
      }
    }

    private void ClearDocument()
    {
      if (this._documentType == null)
        return;
      this.LoaderHost.RemoveService(typeof (CodeTypeDeclaration));
      this._documentType = (CodeTypeDeclaration) null;
      this._documentNamespace = (CodeNamespace) null;
      this._documentCompileUnit = (CodeCompileUnit) null;
      this._rootSerializer = (CodeDomSerializer) null;
      this._typeSerializer = (TypeCodeDomSerializer) null;
    }

    private bool HasRootDesignerAttribute(Type t)
    {
      AttributeCollection attributes = TypeDescriptor.GetAttributes(t);
      for (int index = 0; index < attributes.Count; ++index)
      {
        DesignerAttribute designerAttribute = attributes[index] as DesignerAttribute;
        if (designerAttribute != null)
        {
          Type type = Type.GetType(designerAttribute.DesignerBaseTypeName);
          if (type != (Type) null && type == typeof (IRootDesigner))
            return true;
        }
      }
      return false;
    }

    private void EnsureDocument(IDesignerSerializationManager manager)
    {
      if (this._documentCompileUnit == null)
      {
        this._documentCompileUnit = this.Parse();
        if (this._documentCompileUnit == null)
        {
          Exception exception = (Exception) new NotSupportedException(System.Design.SR.GetString("CustomCodeDomHostLoaderNoLanguageSupport"));
          exception.HelpLink = "CustomCodeDomHostLoaderNoLanguageSupport";
          throw exception;
        }
      }
      if (this._documentType == null)
      {
        ArrayList arrayList = (ArrayList) null;
        bool flag1 = true;
        if (this._documentCompileUnit.UserData[(object) typeof (InvalidOperationException)] != null)
        {
          InvalidOperationException operationException = this._documentCompileUnit.UserData[(object) typeof (InvalidOperationException)] as InvalidOperationException;
          if (operationException != null)
          {
            this._documentCompileUnit = (CodeCompileUnit) null;
            throw operationException;
          }
        }
        foreach (CodeNamespace codeNamespace in (CollectionBase) this._documentCompileUnit.Namespaces)
        {
          foreach (CodeTypeDeclaration codeTypeDeclaration in (CollectionBase) codeNamespace.Types)
          {
            Type type1 = (Type) null;
            foreach (CodeTypeReference typeref in (CollectionBase) codeTypeDeclaration.BaseTypes)
            {
              Type type2 = this.LoaderHost.GetType(CodeDomSerializerBase.GetTypeNameFromCodeTypeReference(manager, typeref));
              if (type2 != (Type) null && !type2.IsInterface)
              {
                type1 = type2;
                break;
              }
              else if (type2 == (Type) null)
              {
                if (arrayList == null)
                  arrayList = new ArrayList();
                arrayList.Add((object) System.Design.SR.GetString("CustomCodeDomHostLoaderDocumentFailureTypeNotFound", (object) codeTypeDeclaration.Name, (object) typeref.BaseType));
              }
            }
            if (type1 != (Type) null)
            {
              bool flag2 = false;
              foreach (Attribute attribute in TypeDescriptor.GetAttributes(type1))
              {
                if (attribute is RootDesignerSerializerAttribute)
                {
                  RootDesignerSerializerAttribute serializerAttribute = (RootDesignerSerializerAttribute) attribute;
                  string serializerBaseTypeName = serializerAttribute.SerializerBaseTypeName;
                  if (serializerBaseTypeName != null && this.LoaderHost.GetType(serializerBaseTypeName) == typeof (CodeDomSerializer))
                  {
                    Type type2 = this.LoaderHost.GetType(serializerAttribute.SerializerTypeName);
                    if (type2 != (Type) null && type2 != typeof (RootCodeDomSerializer))
                    {
                      flag2 = true;
                      if (flag1)
                      {
                        this._rootSerializer = (CodeDomSerializer) Activator.CreateInstance(type2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, (object[]) null, (CultureInfo) null);
                        break;
                      }
                      else
                        throw new InvalidOperationException(System.Design.SR.GetString("CustomCodeDomHostLoaderSerializerTypeNotFirstType", new object[1]
                        {
                          (object) codeTypeDeclaration.Name
                        }));
                    }
                  }
                }
              }
              if (this._rootSerializer == null && this.HasRootDesignerAttribute(type1))
              {
                this._typeSerializer = manager.GetSerializer(type1, typeof (TypeCodeDomSerializer)) as TypeCodeDomSerializer;
                if (!flag1 && this._typeSerializer != null)
                {
                  this._typeSerializer = (TypeCodeDomSerializer) null;
                  this._documentCompileUnit = (CodeCompileUnit) null;
                  throw new InvalidOperationException(System.Design.SR.GetString("CustomCodeDomHostLoaderSerializerTypeNotFirstType", new object[1]
                  {
                    (object) codeTypeDeclaration.Name
                  }));
                }
              }
              if (this._rootSerializer == null && this._typeSerializer == null)
              {
                if (arrayList == null)
                  arrayList = new ArrayList();
                if (flag2)
                  arrayList.Add((object) System.Design.SR.GetString("CustomCodeDomHostLoaderDocumentFailureTypeDesignerNotInstalled", (object) codeTypeDeclaration.Name, (object) type1.FullName));
                else
                  arrayList.Add((object) System.Design.SR.GetString("CustomCodeDomHostLoaderDocumentFailureTypeNotDesignable", (object) codeTypeDeclaration.Name, (object) type1.FullName));
              }
            }
            if (this._rootSerializer != null || this._typeSerializer != null)
            {
              this._documentNamespace = codeNamespace;
              this._documentType = codeTypeDeclaration;
              break;
            }
            else
              flag1 = false;
          }
          if (this._documentType != null)
            break;
        }
        if (this._documentType == null)
        {
          this._documentCompileUnit = (CodeCompileUnit) null;
          Exception exception;
          if (arrayList != null)
          {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in arrayList)
            {
              stringBuilder.Append("\r\n");
              stringBuilder.Append(str);
            }
            exception = (Exception) new InvalidOperationException(System.Design.SR.GetString("CustomCodeDomHostLoaderNoRootSerializerWithFailures", new object[1]
            {
              (object) ((object) stringBuilder).ToString()
            }));
            exception.HelpLink = "CustomCodeDomHostLoaderNoRootSerializer";
          }
          else
          {
            exception = (Exception) new InvalidOperationException(System.Design.SR.GetString("CustomCodeDomHostLoaderNoRootSerializer"));
            exception.HelpLink = "CustomCodeDomHostLoaderNoRootSerializer";
          }
          throw exception;
        }
        else
          this.LoaderHost.AddService(typeof (CodeTypeDeclaration), (object) this._documentType);
      }
      CustomCodeDomHostLoader.codemarkers.CodeMarker(7526);
    }

    private bool IntegrateSerializedTree(IDesignerSerializationManager manager, CodeTypeDeclaration newDecl)
    {
      this.EnsureDocument(manager);
      CodeTypeDeclaration codeTypeDeclaration = this._documentType;
      bool caseInsensitive = false;
      bool flag1 = false;
      CodeDomProvider codeDomProvider = this.CodeDomProvider;
      if (codeDomProvider != null)
        caseInsensitive = (codeDomProvider.LanguageOptions & LanguageOptions.CaseInsensitive) != LanguageOptions.None;
      if (!string.Equals(codeTypeDeclaration.Name, newDecl.Name, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
      {
        codeTypeDeclaration.Name = newDecl.Name;
        flag1 = true;
      }
      if (!codeTypeDeclaration.Attributes.Equals((object) newDecl.Attributes))
      {
        codeTypeDeclaration.Attributes = newDecl.Attributes;
        flag1 = true;
      }
      int index1 = 0;
      bool flag2 = false;
      int index2 = 0;
      bool flag3 = false;
      IDictionary dictionary = (IDictionary) new HybridDictionary(codeTypeDeclaration.Members.Count, caseInsensitive);
      int count = codeTypeDeclaration.Members.Count;
      for (int index3 = 0; index3 < count; ++index3)
      {
        CodeTypeMember codeTypeMember = codeTypeDeclaration.Members[index3];
        string str = !(codeTypeMember is CodeConstructor) ? (!(codeTypeMember is CodeTypeConstructor) ? codeTypeMember.Name : ".cctor") : ".ctor";
        dictionary[(object) str] = (object) index3;
        if (codeTypeMember is CodeMemberField)
        {
          if (!flag2)
            index1 = index3;
        }
        else if (index1 > 0)
          flag2 = true;
        if (codeTypeMember is CodeMemberMethod)
        {
          if (!flag3)
            index2 = index3;
        }
        else if (index2 > 0)
          flag3 = true;
      }
      ArrayList arrayList = new ArrayList();
      foreach (CodeTypeMember codeTypeMember1 in (CollectionBase) newDecl.Members)
      {
        string str = !(codeTypeMember1 is CodeConstructor) ? codeTypeMember1.Name : ".ctor";
        object obj = dictionary[(object) str];
        if (obj != null)
        {
          int index3 = (int) obj;
          CodeTypeMember codeTypeMember2 = codeTypeDeclaration.Members[index3];
          if (codeTypeMember2 != codeTypeMember1)
          {
            if (codeTypeMember1 is CodeMemberField)
            {
              if (codeTypeMember2 is CodeMemberField)
              {
                CodeMemberField codeMemberField1 = (CodeMemberField) codeTypeMember2;
                CodeMemberField codeMemberField2 = (CodeMemberField) codeTypeMember1;
                if (!string.Equals(codeMemberField2.Name, codeMemberField1.Name) || codeMemberField2.Attributes != codeMemberField1.Attributes || !CustomCodeDomHostLoader.TypesEqual(codeMemberField2.Type, codeMemberField1.Type))
                  codeTypeDeclaration.Members[index3] = codeTypeMember1;
                else
                  continue;
              }
              else
                arrayList.Add((object) codeTypeMember1);
            }
            else if (codeTypeMember1 is CodeMemberMethod)
            {
              if (codeTypeMember2 is CodeMemberMethod && !(codeTypeMember2 is CodeConstructor))
              {
                CodeMemberMethod codeMemberMethod1 = (CodeMemberMethod) codeTypeMember2;
                CodeMemberMethod codeMemberMethod2 = (CodeMemberMethod) codeTypeMember1;
                codeMemberMethod1.Statements.Clear();
                codeMemberMethod1.Statements.AddRange(codeMemberMethod2.Statements);
              }
            }
            else
              codeTypeDeclaration.Members[index3] = codeTypeMember1;
            flag1 = true;
          }
        }
        else
          arrayList.Add((object) codeTypeMember1);
      }
      foreach (CodeTypeMember codeTypeMember in arrayList)
      {
        if (codeTypeMember is CodeMemberField)
        {
          if (index1 >= codeTypeDeclaration.Members.Count)
            codeTypeDeclaration.Members.Add(codeTypeMember);
          else
            codeTypeDeclaration.Members.Insert(index1, codeTypeMember);
          ++index1;
          ++index2;
          flag1 = true;
        }
        else if (codeTypeMember is CodeMemberMethod)
        {
          if (index2 >= codeTypeDeclaration.Members.Count)
            codeTypeDeclaration.Members.Add(codeTypeMember);
          else
            codeTypeDeclaration.Members.Insert(index2, codeTypeMember);
          ++index2;
          flag1 = true;
        }
        else
        {
          codeTypeDeclaration.Members.Add(codeTypeMember);
          flag1 = true;
        }
      }
      return flag1;
    }

    private void OnComponentRemoved(object sender, ComponentEventArgs e)
    {
      this.RemoveDeclaration(e.Component.Site.Name);
    }

    private void OnComponentRename(object sender, ComponentRenameEventArgs e)
    {
      this.OnComponentRename(e.Component, e.OldName, e.NewName);
    }

    private object OnCreateService(IServiceContainer container, Type serviceType)
    {
      if (serviceType == typeof (ComponentSerializationService))
        return (object) new CodeDomComponentSerializationService((IServiceProvider) this.LoaderHost);
      else
        return (object) null;
    }

    private void RemoveDeclaration(string name)
    {
      if (this._documentType == null)
        return;
      CodeTypeMemberCollection members = this._documentType.Members;
      for (int index = 0; index < members.Count; ++index)
      {
        if (members[index] is CodeMemberField && members[index].Name.Equals(name))
        {
          members.RemoveAt(index);
          break;
        }
      }
    }

    private void ThrowMissingService(Type serviceType)
    {
      Exception exception = (Exception) new InvalidOperationException(System.Design.SR.GetString("BasicDesignerLoaderMissingService", new object[1]
      {
        (object) serviceType.Name
      }));
      exception.HelpLink = "BasicDesignerLoaderMissingService";
      throw exception;
    }

    private static bool TypesEqual(CodeTypeReference typeLeft, CodeTypeReference typeRight)
    {
      if (typeLeft.ArrayRank != typeRight.ArrayRank || !typeLeft.BaseType.Equals(typeRight.BaseType) || typeLeft.TypeArguments != null && typeRight.TypeArguments == null || typeLeft.TypeArguments == null && typeRight.TypeArguments != null)
        return false;
      if (typeLeft.TypeArguments != null && typeRight.TypeArguments != null)
      {
        if (typeLeft.TypeArguments.Count != typeRight.TypeArguments.Count)
          return false;
        for (int index = 0; index < typeLeft.TypeArguments.Count; ++index)
        {
          if (!CustomCodeDomHostLoader.TypesEqual(typeLeft.TypeArguments[index], typeRight.TypeArguments[index]))
            return false;
        }
      }
      if (typeLeft.ArrayRank > 0)
        return CustomCodeDomHostLoader.TypesEqual(typeLeft.ArrayElementType, typeRight.ArrayElementType);
      else
        return true;
    }

    [ProvideProperty("Modifiers", typeof (IComponent))]
    [ProvideProperty("GenerateMember", typeof (IComponent))]
    private class ModifiersExtenderProvider : IExtenderProvider
    {
      private IDesignerHost _host;

      public ModifiersExtenderProvider():base()
      {
      }

      public bool CanExtend(object o)
      {
        IComponent c = o as IComponent;
        if (c == null)
          return false;
        IComponent baseComponent = this.GetBaseComponent(c);
        return o != baseComponent && TypeDescriptor.GetAttributes(o)[typeof (InheritanceAttribute)].Equals((object) InheritanceAttribute.NotInherited);
      }

      private IComponent GetBaseComponent(IComponent c)
      {
        IComponent component = (IComponent) null;
        if (c == null)
          return (IComponent) null;
        if (this._host == null)
        {
          ISite site = c.Site;
          if (site != null)
            this._host = (IDesignerHost) site.GetService(typeof (IDesignerHost));
        }
        if (this._host != null)
          component = this._host.RootComponent;
        return component;
      }

      [HelpKeyword("Designer_GenerateMember")]
      [Category("Design")]
      [DefaultValue(true)]
      [DesignOnly(true)]
      [SRDescription("CustomCodeDomHostLoaderPropGenerateMember")]
      public bool GetGenerateMember(IComponent comp)
      {
        ISite site = comp.Site;
        if (site != null)
        {
          IDictionaryService dictionaryService = (IDictionaryService) site.GetService(typeof (IDictionaryService));
          if (dictionaryService != null)
          {
            object obj = dictionaryService.GetValue((object) "GenerateMember");
            if (obj is bool)
              return (bool) obj;
          }
        }
        return true;
      }

      [SRDescription("CustomCodeDomHostLoaderPropModifiers")]
      [DefaultValue(MemberAttributes.Private)]
      [DesignOnly(true)]
      [TypeConverter(typeof (CustomCodeDomHostLoader.ModifierConverter))]
      [Category("Design")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [HelpKeyword("Designer_Modifiers")]
      public MemberAttributes GetModifiers(IComponent comp)
      {
        ISite site = comp.Site;
        if (site != null)
        {
          IDictionaryService dictionaryService = (IDictionaryService) site.GetService(typeof (IDictionaryService));
          if (dictionaryService != null)
          {
            object obj = dictionaryService.GetValue((object) "Modifiers");
            if (obj is MemberAttributes)
              return (MemberAttributes) obj;
          }
        }
        PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties((object) comp)["DefaultModifiers"];
        if (propertyDescriptor != null && propertyDescriptor.PropertyType == typeof (MemberAttributes))
          return (MemberAttributes) propertyDescriptor.GetValue((object) comp);
        else
          return MemberAttributes.Private;
      }

      public void SetGenerateMember(IComponent comp, bool generate)
      {
        ISite site = comp.Site;
        if (site == null)
          return;
        IDictionaryService dictionaryService = (IDictionaryService) site.GetService(typeof (IDictionaryService));
        bool generateMember = this.GetGenerateMember(comp);
        if (dictionaryService != null)
          dictionaryService.SetValue((object) "GenerateMember", (object) generate);
        if (!generateMember || generate)
          return;
        CodeTypeDeclaration codeTypeDeclaration = site.GetService(typeof (CodeTypeDeclaration)) as CodeTypeDeclaration;
        string name = site.Name;
        if (codeTypeDeclaration == null || name == null)
          return;
        foreach (CodeTypeMember codeTypeMember in (CollectionBase) codeTypeDeclaration.Members)
        {
          CodeMemberField codeMemberField = codeTypeMember as CodeMemberField;
          if (codeMemberField != null && codeMemberField.Name.Equals(name))
          {
            codeTypeDeclaration.Members.Remove((CodeTypeMember) codeMemberField);
            break;
          }
        }
      }

      public void SetModifiers(IComponent comp, MemberAttributes modifiers)
      {
        ISite site = comp.Site;
        if (site == null)
          return;
        IDictionaryService dictionaryService = (IDictionaryService) site.GetService(typeof (IDictionaryService));
        if (dictionaryService == null)
          return;
        dictionaryService.SetValue((object) "Modifiers", (object) modifiers);
      }
    }

    [ProvideProperty("Modifiers", typeof (IComponent))]
    private class ModifiersInheritedExtenderProvider : IExtenderProvider
    {
      private IDesignerHost _host;

      public ModifiersInheritedExtenderProvider():base()
      {
      }

      public bool CanExtend(object o)
      {
        IComponent c = o as IComponent;
        if (c == null)
          return false;
        IComponent baseComponent = this.GetBaseComponent(c);
        return o != baseComponent && !TypeDescriptor.GetAttributes(o)[typeof (InheritanceAttribute)].Equals((object) InheritanceAttribute.NotInherited);
      }

      private IComponent GetBaseComponent(IComponent c)
      {
        IComponent component = (IComponent) null;
        if (c == null)
          return (IComponent) null;
        if (this._host == null)
        {
          ISite site = c.Site;
          if (site != null)
            this._host = (IDesignerHost) site.GetService(typeof (IDesignerHost));
        }
        if (this._host != null)
          component = this._host.RootComponent;
        return component;
      }

      [DesignOnly(true)]
      [TypeConverter(typeof (CustomCodeDomHostLoader.ModifierConverter))]
      [Category("Design")]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [SRDescription("CustomCodeDomHostLoaderPropModifiers")]
      [DefaultValue(MemberAttributes.Private)]
      public MemberAttributes GetModifiers(IComponent comp)
      {
        Type type = this.GetBaseComponent(comp).GetType();
        ISite site = comp.Site;
        if (site != null)
        {
          string name = site.Name;
          if (name != null)
          {
            FieldInfo field = TypeDescriptor.GetReflectionType(type).GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != (FieldInfo) null)
            {
              if (field.IsPrivate)
                return MemberAttributes.Private;
              if (field.IsPublic)
                return MemberAttributes.Public;
              if (field.IsFamily)
                return MemberAttributes.Family;
              if (field.IsAssembly)
                return MemberAttributes.Assembly;
              if (field.IsFamilyOrAssembly)
                return MemberAttributes.FamilyOrAssembly;
              if (field.IsFamilyAndAssembly)
                return MemberAttributes.FamilyAndAssembly;
            }
            else
            {
              PropertyInfo property = TypeDescriptor.GetReflectionType(type).GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
              if (property != (PropertyInfo) null)
              {
                MethodInfo[] accessors = property.GetAccessors(true);
                if (accessors != null && accessors.Length > 0)
                {
                  MethodInfo methodInfo = accessors[0];
                  if (methodInfo != (MethodInfo) null && !methodInfo.IsPrivate)
                  {
                    if (methodInfo.IsPublic)
                      return MemberAttributes.Public;
                    if (methodInfo.IsFamily)
                      return MemberAttributes.Family;
                    if (methodInfo.IsAssembly)
                      return MemberAttributes.Assembly;
                    if (methodInfo.IsFamilyOrAssembly)
                      return MemberAttributes.FamilyOrAssembly;
                    if (methodInfo.IsFamilyAndAssembly)
                      return MemberAttributes.FamilyAndAssembly;
                  }
                }
              }
            }
          }
        }
        return MemberAttributes.Private;
      }
    }

    private class ModifierConverter : TypeConverter
    {
      public ModifierConverter():base()
      {
      }

      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
        return this.GetConverter(context).CanConvertFrom(context, sourceType);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
        return this.GetConverter(context).CanConvertTo(context, destinationType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
        return this.GetConverter(context).ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
        return this.GetConverter(context).ConvertTo(context, culture, value, destinationType);
      }

      public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
      {
        return this.GetConverter(context).CreateInstance(context, propertyValues);
      }

      private TypeConverter GetConverter(ITypeDescriptorContext context)
      {
        TypeConverter typeConverter = (TypeConverter) null;
        if (context != null)
        {
          CodeDomProvider codeDomProvider = (CodeDomProvider) context.GetService(typeof (CodeDomProvider));
          if (codeDomProvider != null)
            typeConverter = codeDomProvider.GetConverter(typeof (MemberAttributes));
        }
        if (typeConverter == null)
          typeConverter = TypeDescriptor.GetConverter(typeof (MemberAttributes));
        return typeConverter;
      }

      public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
      {
        return this.GetConverter(context).GetCreateInstanceSupported(context);
      }

      public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
      {
        return this.GetConverter(context).GetProperties(context, value, attributes);
      }

      public override bool GetPropertiesSupported(ITypeDescriptorContext context)
      {
        return this.GetConverter(context).GetPropertiesSupported(context);
      }

      public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
      {
        TypeConverter.StandardValuesCollection valuesCollection = this.GetConverter(context).GetStandardValues(context);
        if (valuesCollection != null && valuesCollection.Count > 0)
        {
          bool flag = false;
          foreach (MemberAttributes memberAttributes in valuesCollection)
          {
            if ((memberAttributes & MemberAttributes.AccessMask) == (MemberAttributes) 0)
            {
              flag = true;
              break;
            }
          }
          if (flag)
          {
            ArrayList arrayList = new ArrayList(valuesCollection.Count);
            foreach (MemberAttributes memberAttributes in valuesCollection)
            {
              if ((memberAttributes & MemberAttributes.AccessMask) != (MemberAttributes) 0 && memberAttributes != MemberAttributes.AccessMask)
                arrayList.Add((object) memberAttributes);
            }
            valuesCollection = new TypeConverter.StandardValuesCollection((ICollection) arrayList);
          }
        }
        return valuesCollection;
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
      {
        return this.GetConverter(context).GetStandardValuesExclusive(context);
      }

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
      {
        return this.GetConverter(context).GetStandardValuesSupported(context);
      }

      public override bool IsValid(ITypeDescriptorContext context, object value)
      {
        return this.GetConverter(context).IsValid(context, value);
      }
    }
  }

    class CodeMarkers
  {
    public static readonly CodeMarkers Instance;
    private CodeMarkers.State state;
    private const string AtomName = "VSCodeMarkersEnabled";
    private const string DllName = "Microsoft.Internal.Performance.CodeMarkers.dll";

    public bool IsEnabled
    {
      get
      {
        return this.state == CodeMarkers.State.Enabled;
      }
    }

    static CodeMarkers()
    {
      CodeMarkers.Instance = new CodeMarkers();
    }

    private CodeMarkers(): base()
    {      
      this.state = (int) CodeMarkers.NativeMethods.FindAtom("VSCodeMarkersEnabled") != 0 ? CodeMarkers.State.Enabled : CodeMarkers.State.Disabled;
    }

    public bool CodeMarker(int nTimerID)
    {
      if (!this.IsEnabled)
        return false;
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker(nTimerID, (byte[]) null, 0);
      }
      catch (DllNotFoundException ex)
      {
        this.state = CodeMarkers.State.DisabledDueToDllImportException;
        return false;
      }
      return true;
    }

    public bool CodeMarkerEx(int nTimerID, byte[] aBuff)
    {
      if (!this.IsEnabled)
        return false;
      if (aBuff == null)
        throw new ArgumentNullException("aBuff");
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker(nTimerID, aBuff, aBuff.Length);
      }
      catch (DllNotFoundException ex)
      {
        this.state = CodeMarkers.State.DisabledDueToDllImportException;
        return false;
      }
      return true;
    }

    public bool CodeMarkerEx(int nTimerID, Guid guidData)
    {
      return this.CodeMarkerEx(nTimerID, guidData.ToByteArray());
    }

    public bool CodeMarkerEx(int nTimerID, string stringData)
    {
      return this.CodeMarkerEx(nTimerID, Encoding.Unicode.GetBytes(stringData));
    }

    public bool CodeMarkerEx(int nTimerID, uint uintData)
    {
      return this.CodeMarkerEx(nTimerID, BitConverter.GetBytes(uintData));
    }

    public bool CodeMarkerEx(int nTimerID, ulong ulongData)
    {
      return this.CodeMarkerEx(nTimerID, BitConverter.GetBytes(ulongData));
    }

    private enum State
    {
      Enabled,
      Disabled,
      DisabledDueToDllImportException,
      DisabledViaRegistryCheck,
    }

    private static class NativeMethods
    {
      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
      public static void DllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static ushort FindAtom([MarshalAs(UnmanagedType.LPWStr)] string lpString);
    }
  }

    class RootCodeDomSerializer : ComponentCodeDomSerializer
  {
    private IDictionary nameTable;
    private IDictionary statementTable;
    private CodeMemberMethod initMethod;
    private bool containerRequired;
    private static readonly Attribute[] designTimeProperties;
    private static readonly Attribute[] runTimeProperties;

    public string ContainerName
    {
      get
      {
        return "components";
      }
    }

    public bool ContainerRequired
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.containerRequired;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.containerRequired = value;
      }
    }

    public string InitMethodName
    {
      get
      {
        return "InitializeComponent";
      }
    }

    static RootCodeDomSerializer()
    {
      RootCodeDomSerializer.designTimeProperties = new Attribute[1]
      {
        (Attribute) DesignOnlyAttribute.Yes
      };
      RootCodeDomSerializer.runTimeProperties = new Attribute[1]
      {
        (Attribute) DesignOnlyAttribute.No
      };
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public RootCodeDomSerializer():base()
    {
    }

    public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
    {
      if (manager == null || codeObject == null)
        throw new ArgumentNullException(manager == null ? "manager" : "codeObject");
      object obj1 = (object) null;
      using (CodeDomSerializerBase.TraceScope("RootCodeDomSerializer::Deserialize"))
      {
        if (!(codeObject is CodeTypeDeclaration))
        {
          throw new ArgumentException(System.Design.SR.GetString("SerializerBadElementType", new object[1]
          {
            (object) typeof (CodeTypeDeclaration).FullName
          }));
        }
        else
        {
          bool flag1 = false;
          CodeDomProvider codeDomProvider = manager.GetService(typeof (CodeDomProvider)) as CodeDomProvider;
          if (codeDomProvider != null)
            flag1 = (codeDomProvider.LanguageOptions & LanguageOptions.CaseInsensitive) != LanguageOptions.None;
          CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration) codeObject;
          CodeTypeReference codeTypeReference = (CodeTypeReference) null;
          Type type1 = (Type) null;
          foreach (CodeTypeReference typeref in (CollectionBase) codeTypeDeclaration.BaseTypes)
          {
            Type type2 = manager.GetType(CodeDomSerializerBase.GetTypeNameFromCodeTypeReference(manager, typeref));
            if (type2 != (Type) null && !type2.IsInterface)
            {
              codeTypeReference = typeref;
              type1 = type2;
              break;
            }
          }
          if (type1 == (Type) null)
          {
            Exception exception = (Exception) new SerializationException(System.Design.SR.GetString("SerializerTypeNotFound", new object[1]
            {
              (object) codeTypeReference.BaseType
            }));
            exception.HelpLink = "SerializerTypeNotFound";
            throw exception;
          }
          else if (type1.IsAbstract)
          {
            Exception exception = (Exception) new SerializationException(System.Design.SR.GetString("SerializerTypeAbstract", new object[1]
            {
              (object) type1.FullName
            }));
            exception.HelpLink = "SerializerTypeAbstract";
            throw exception;
          }
          else
          {
            // ISSUE: method pointer
            ResolveNameEventHandler nameEventHandler = new ResolveNameEventHandler((object) this, __methodptr(OnResolveName));
            manager.ResolveName += nameEventHandler;
            if (!(manager is DesignerSerializationManager))
              manager.AddSerializationProvider((IDesignerSerializationProvider) new CodeDomSerializationProvider());
            obj1 = manager.CreateInstance(type1, (ICollection) null, codeTypeDeclaration.Name, true);
            this.nameTable = (IDictionary) new HybridDictionary(codeTypeDeclaration.Members.Count, flag1);
            this.statementTable = (IDictionary) new HybridDictionary(codeTypeDeclaration.Members.Count, flag1);
            this.initMethod = (CodeMemberMethod) null;
            RootContext rootContext = new RootContext((CodeExpression) new CodeThisReferenceExpression(), obj1);
            manager.Context.Push((object) rootContext);
            try
            {
              foreach (CodeTypeMember codeTypeMember in (CollectionBase) codeTypeDeclaration.Members)
              {
                if (codeTypeMember is CodeMemberField)
                {
                  if (string.Compare(codeTypeMember.Name, codeTypeDeclaration.Name, flag1, CultureInfo.InvariantCulture) != 0)
                    this.nameTable[(object) codeTypeMember.Name] = (object) codeTypeMember;
                }
                else if (this.initMethod == null && codeTypeMember is CodeMemberMethod)
                {
                  CodeMemberMethod codeMemberMethod = (CodeMemberMethod) codeTypeMember;
                  if (string.Compare(codeMemberMethod.Name, this.InitMethodName, flag1, CultureInfo.InvariantCulture) == 0 && codeMemberMethod.Parameters.Count == 0)
                    this.initMethod = codeMemberMethod;
                }
              }
              if (this.initMethod != null)
              {
                foreach (CodeStatement codeStatement in (CollectionBase) this.initMethod.Statements)
                {
                  CodeVariableDeclarationStatement declarationStatement = codeStatement as CodeVariableDeclarationStatement;
                  if (declarationStatement != null)
                    this.nameTable[(object) declarationStatement.Name] = (object) codeStatement;
                }
              }
              if (this.nameTable[(object) codeTypeDeclaration.Name] != null)
                this.nameTable[(object) codeTypeDeclaration.Name] = obj1;
              if (this.initMethod != null)
                this.FillStatementTable(this.initMethod, codeTypeDeclaration.Name);
              PropertyDescriptor propertyDescriptor = manager.Properties["SupportsStatementGeneration"];
              if (propertyDescriptor != null && propertyDescriptor.PropertyType == typeof (bool) && (bool) propertyDescriptor.GetValue((object) manager))
              {
                foreach (string str in (IEnumerable) this.nameTable.Keys)
                {
                  CodeDomSerializerBase.OrderedCodeStatementCollection statementCollection = (CodeDomSerializerBase.OrderedCodeStatementCollection) this.statementTable[(object) str];
                  if (statementCollection != null)
                  {
                    bool flag2 = false;
                    foreach (CodeObject codeObject1 in (CollectionBase) statementCollection)
                    {
                      object obj2 = codeObject1.UserData[(object) "GeneratedStatement"];
                      if (obj2 == null || !(obj2 is bool) || !(bool) obj2)
                      {
                        flag2 = true;
                        break;
                      }
                    }
                    if (!flag2)
                      this.statementTable.Remove((object) str);
                  }
                }
              }
              IContainer container = (IContainer) manager.GetService(typeof (IContainer));
              if (container != null)
              {
                foreach (object obj2 in (ReadOnlyCollectionBase) container.Components)
                  this.DeserializePropertiesFromResources(manager, obj2, RootCodeDomSerializer.designTimeProperties);
              }
              object[] objArray = new object[this.statementTable.Values.Count];
              this.statementTable.Values.CopyTo((Array) objArray, 0);
              Array.Sort((Array) objArray, (IComparer) RootCodeDomSerializer.StatementOrderComparer.Default);
              foreach (CodeDomSerializerBase.OrderedCodeStatementCollection statementCollection in objArray)
              {
                string name = statementCollection.Name;
                if (name != null && !name.Equals(codeTypeDeclaration.Name))
                  this.DeserializeName(manager, name);
              }
              CodeStatementCollection statementCollection1 = (CodeStatementCollection) this.statementTable[(object) codeTypeDeclaration.Name];
              if (statementCollection1 != null)
              {
                if (statementCollection1.Count > 0)
                {
                  foreach (CodeStatement statement in (CollectionBase) statementCollection1)
                    this.DeserializeStatement(manager, statement);
                }
              }
            }
            finally
            {
              manager.ResolveName -= nameEventHandler;
              this.initMethod = (CodeMemberMethod) null;
              this.nameTable = (IDictionary) null;
              this.statementTable = (IDictionary) null;
              manager.Context.Pop();
            }
          }
        }
      }
      return obj1;
    }

    public override object Serialize(IDesignerSerializationManager manager, object value)
    {
      if (manager == null || value == null)
        throw new ArgumentNullException(manager == null ? "manager" : "value");
      CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(manager.GetName(value));
      RootContext rootContext = new RootContext((CodeExpression) new CodeThisReferenceExpression(), value);
      using (CodeDomSerializerBase.TraceScope("RootCodeDomSerializer::Serialize"))
      {
        codeTypeDeclaration.BaseTypes.Add(value.GetType());
        this.containerRequired = false;
        manager.Context.Push((object) rootContext);
        manager.Context.Push((object) this);
        manager.Context.Push((object) codeTypeDeclaration);
        if (!(manager is DesignerSerializationManager))
          manager.AddSerializationProvider((IDesignerSerializationProvider) new CodeDomSerializationProvider());
        try
        {
          if (value is IComponent)
          {
            ISite site = ((IComponent) value).Site;
            if (site != null)
            {
              ICollection statementOwners = (ICollection) site.Container.Components;
              StatementContext statementContext = new StatementContext();
              statementContext.StatementCollection.Populate(statementOwners);
              manager.Context.Push((object) statementContext);
              try
              {
                foreach (IComponent component in (IEnumerable) statementOwners)
                {
                  if (component != value && !this.IsSerialized(manager, (object) component))
                  {
                    if (this.GetSerializer(manager, (object) component) != null)
                      this.SerializeToExpression(manager, (object) component);
                    else
                      manager.ReportError((object) System.Design.SR.GetString("SerializerNoSerializerForComponent", new object[1]
                      {
                        (object) component.GetType().FullName
                      }));
                  }
                }
                manager.Context.Push(value);
                try
                {
                  if (this.GetSerializer(manager, value) != null && !this.IsSerialized(manager, value))
                    this.SerializeToExpression(manager, value);
                  else
                    manager.ReportError((object) System.Design.SR.GetString("SerializerNoSerializerForComponent", new object[1]
                    {
                      (object) value.GetType().FullName
                    }));
                }
                finally
                {
                  manager.Context.Pop();
                }
              }
              finally
              {
                manager.Context.Pop();
              }
              CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
              codeMemberMethod.Name = this.InitMethodName;
              codeMemberMethod.Attributes = MemberAttributes.Private;
              codeTypeDeclaration.Members.Add((CodeTypeMember) codeMemberMethod);
              ArrayList elements = new ArrayList();
              foreach (object index in (IEnumerable) statementOwners)
              {
                if (index != value)
                  elements.Add((object) statementContext.StatementCollection[index]);
              }
              if (statementContext.StatementCollection[value] != null)
                elements.Add((object) statementContext.StatementCollection[value]);
              if (this.ContainerRequired)
                this.SerializeContainerDeclaration(manager, codeMemberMethod.Statements);
              this.SerializeElementsToStatements(elements, codeMemberMethod.Statements);
            }
          }
        }
        finally
        {
          manager.Context.Pop();
          manager.Context.Pop();
          manager.Context.Pop();
        }
      }
      return (object) codeTypeDeclaration;
    }

    private void AddStatement(string name, CodeStatement statement)
    {
      CodeDomSerializerBase.OrderedCodeStatementCollection statementCollection = (CodeDomSerializerBase.OrderedCodeStatementCollection) this.statementTable[(object) name];
      if (statementCollection == null)
      {
        statementCollection = new CodeDomSerializerBase.OrderedCodeStatementCollection();
        statementCollection.Order = this.statementTable.Count;
        statementCollection.Name = name;
        this.statementTable[(object) name] = (object) statementCollection;
      }
      statementCollection.Add(statement);
    }

    private object DeserializeName(IDesignerSerializationManager manager, string name)
    {
      string typeName = (string) null;
      object component1 = this.nameTable[(object) name];
      using (CodeDomSerializerBase.TraceScope("RootCodeDomSerializer::DeserializeName"))
      {
        CodeMemberField codeMemberField = (CodeMemberField) null;
        CodeObject codeObject = component1 as CodeObject;
        if (codeObject != null)
        {
          component1 = (object) null;
          this.nameTable[(object) name] = (object) null;
          if (codeObject is CodeVariableDeclarationStatement)
          {
            CodeVariableDeclarationStatement declarationStatement = (CodeVariableDeclarationStatement) codeObject;
            typeName = CodeDomSerializerBase.GetTypeNameFromCodeTypeReference(manager, declarationStatement.Type);
          }
          else if (codeObject is CodeMemberField)
          {
            codeMemberField = (CodeMemberField) codeObject;
            typeName = CodeDomSerializerBase.GetTypeNameFromCodeTypeReference(manager, codeMemberField.Type);
          }
        }
        else
        {
          if (component1 != null)
            return component1;
          IContainer container = (IContainer) manager.GetService(typeof (IContainer));
          if (container != null)
          {
            IComponent component2 = container.Components[name];
            if (component2 != null)
            {
              typeName = component2.GetType().FullName;
              this.nameTable[(object) name] = (object) component2;
            }
          }
        }
        if (name.Equals(this.ContainerName))
        {
          IContainer container = (IContainer) manager.GetService(typeof (IContainer));
          if (container != null)
            component1 = (object) container;
        }
        else if (typeName != null)
        {
          Type type = manager.GetType(typeName);
          if (type == (Type) null)
          {
            manager.ReportError((object) new SerializationException(System.Design.SR.GetString("SerializerTypeNotFound", new object[1]
            {
              (object) typeName
            })));
          }
          else
          {
            CodeStatementCollection statementCollection = (CodeStatementCollection) this.statementTable[(object) name];
            if (statementCollection != null && statementCollection.Count > 0)
            {
              CodeDomSerializer codeDomSerializer = (CodeDomSerializer) manager.GetSerializer(type, typeof (CodeDomSerializer));
              if (codeDomSerializer == null)
              {
                manager.ReportError((object) System.Design.SR.GetString("SerializerNoSerializerForComponent", new object[1]
                {
                  (object) type.FullName
                }));
              }
              else
              {
                try
                {
                  component1 = codeDomSerializer.Deserialize(manager, (object) statementCollection);
                  if (component1 != null)
                  {
                    if (codeMemberField != null)
                    {
                      PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(component1)["Modifiers"];
                      if (propertyDescriptor != null)
                      {
                        if (propertyDescriptor.PropertyType == typeof (MemberAttributes))
                        {
                          MemberAttributes memberAttributes = codeMemberField.Attributes & MemberAttributes.AccessMask;
                          propertyDescriptor.SetValue(component1, (object) memberAttributes);
                        }
                      }
                    }
                  }
                }
                catch (Exception ex)
                {
                  manager.ReportError((object) ex);
                }
              }
            }
          }
        }
        this.nameTable[(object) name] = component1;
      }
      return component1;
    }

    private void FillStatementTable(CodeMemberMethod method, string className)
    {
      using (CodeDomSerializerBase.TraceScope("RootCodeDomSerializer::FillStatementTable"))
      {
        IEnumerator enumerator = method.Statements.GetEnumerator();
        try
        {
label_45:
          while (enumerator.MoveNext())
          {
            CodeStatement statement = (CodeStatement) enumerator.Current;
            CodeExpression codeExpression = (CodeExpression) null;
            if (statement is CodeAssignStatement)
              codeExpression = ((CodeAssignStatement) statement).Left;
            else if (statement is CodeAttachEventStatement)
              codeExpression = (CodeExpression) ((CodeAttachEventStatement) statement).Event;
            else if (statement is CodeRemoveEventStatement)
              codeExpression = (CodeExpression) ((CodeRemoveEventStatement) statement).Event;
            else if (statement is CodeExpressionStatement)
              codeExpression = ((CodeExpressionStatement) statement).Expression;
            else if (statement is CodeVariableDeclarationStatement)
            {
              CodeVariableDeclarationStatement declarationStatement = (CodeVariableDeclarationStatement) statement;
              if (declarationStatement.InitExpression != null && this.nameTable.Contains((object) declarationStatement.Name))
                this.AddStatement(declarationStatement.Name, (CodeStatement) declarationStatement);
              codeExpression = (CodeExpression) null;
            }
            if (codeExpression != null)
            {
              while (true)
              {
                while (!(codeExpression is CodeCastExpression))
                {
                  if (codeExpression is CodeDelegateCreateExpression)
                    codeExpression = ((CodeDelegateCreateExpression) codeExpression).TargetObject;
                  else if (codeExpression is CodeDelegateInvokeExpression)
                    codeExpression = ((CodeDelegateInvokeExpression) codeExpression).TargetObject;
                  else if (codeExpression is CodeDirectionExpression)
                    codeExpression = ((CodeDirectionExpression) codeExpression).Expression;
                  else if (codeExpression is CodeEventReferenceExpression)
                    codeExpression = ((CodeEventReferenceExpression) codeExpression).TargetObject;
                  else if (codeExpression is CodeMethodInvokeExpression)
                    codeExpression = (CodeExpression) ((CodeMethodInvokeExpression) codeExpression).Method;
                  else if (codeExpression is CodeMethodReferenceExpression)
                    codeExpression = ((CodeMethodReferenceExpression) codeExpression).TargetObject;
                  else if (codeExpression is CodeArrayIndexerExpression)
                    codeExpression = ((CodeArrayIndexerExpression) codeExpression).TargetObject;
                  else if (codeExpression is CodeFieldReferenceExpression)
                  {
                    CodeFieldReferenceExpression referenceExpression = (CodeFieldReferenceExpression) codeExpression;
                    if (referenceExpression.TargetObject is CodeThisReferenceExpression)
                    {
                      this.AddStatement(referenceExpression.FieldName, statement);
                      goto label_45;
                    }
                    else
                      codeExpression = referenceExpression.TargetObject;
                  }
                  else if (codeExpression is CodePropertyReferenceExpression)
                  {
                    CodePropertyReferenceExpression referenceExpression = (CodePropertyReferenceExpression) codeExpression;
                    if (referenceExpression.TargetObject is CodeThisReferenceExpression && this.nameTable.Contains((object) referenceExpression.PropertyName))
                    {
                      this.AddStatement(referenceExpression.PropertyName, statement);
                      goto label_45;
                    }
                    else
                      codeExpression = referenceExpression.TargetObject;
                  }
                  else if (codeExpression is CodeVariableReferenceExpression)
                  {
                    CodeVariableReferenceExpression referenceExpression = (CodeVariableReferenceExpression) codeExpression;
                    if (this.nameTable.Contains((object) referenceExpression.VariableName))
                    {
                      this.AddStatement(referenceExpression.VariableName, statement);
                      goto label_45;
                    }
                    else
                      goto label_45;
                  }
                  else if (codeExpression is CodeThisReferenceExpression || codeExpression is CodeBaseReferenceExpression)
                  {
                    this.AddStatement(className, statement);
                    goto label_45;
                  }
                  else
                    goto label_45;
                }
                codeExpression = ((CodeCastExpression) codeExpression).Expression;
              }
            }
          }
        }
        finally
        {
          IDisposable disposable = enumerator as IDisposable;
          if (disposable != null)
            disposable.Dispose();
        }
      }
    }

    private string GetMethodName(object statement)
    {
      string str = (string) null;
      while (str == null)
      {
        if (statement is CodeExpressionStatement)
          statement = (object) ((CodeExpressionStatement) statement).Expression;
        else if (statement is CodeMethodInvokeExpression)
          statement = (object) ((CodeMethodInvokeExpression) statement).Method;
        else if (statement is CodeMethodReferenceExpression)
          return ((CodeMethodReferenceExpression) statement).MethodName;
        else
          break;
      }
      return str;
    }

    private void OnResolveName(object sender, ResolveNameEventArgs e)
    {
      using (CodeDomSerializerBase.TraceScope("RootCodeDomSerializer::OnResolveName"))
      {
        if (e.Value != null)
          return;
        object obj = this.DeserializeName((IDesignerSerializationManager) sender, e.Name);
        e.Value = obj;
      }
    }

    private void SerializeContainerDeclaration(IDesignerSerializationManager manager, CodeStatementCollection statements)
    {
      CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration) manager.Context[typeof (CodeTypeDeclaration)];
      if (codeTypeDeclaration == null)
        return;
      CodeMemberField codeMemberField = new CodeMemberField(new CodeTypeReference(typeof (IContainer)), this.ContainerName);
      codeMemberField.Attributes = MemberAttributes.Private;
      codeTypeDeclaration.Members.Add((CodeTypeMember) codeMemberField);
      CodeTypeReference codeTypeReference = new CodeTypeReference(typeof (Container));
      CodeObjectCreateExpression createExpression = new CodeObjectCreateExpression();
      createExpression.CreateType = codeTypeReference;
      CodeAssignStatement codeAssignStatement = new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), this.ContainerName), (CodeExpression) createExpression);
      statements.Add((CodeStatement) codeAssignStatement);
    }

    private void SerializeElementsToStatements(ArrayList elements, CodeStatementCollection statements)
    {
      ArrayList arrayList1 = new ArrayList();
      ArrayList arrayList2 = new ArrayList();
      ArrayList arrayList3 = new ArrayList();
      ArrayList arrayList4 = new ArrayList();
      ArrayList arrayList5 = new ArrayList();
      foreach (object obj in elements)
      {
        if (obj is CodeAssignStatement && ((CodeAssignStatement) obj).Left is CodeFieldReferenceExpression)
          arrayList4.Add(obj);
        else if (obj is CodeVariableDeclarationStatement)
          arrayList3.Add(obj);
        else if (obj is CodeStatement)
        {
          string str = ((CodeObject) obj).UserData[(object) "statement-ordering"] as string;
          if (str != null)
          {
            switch (str)
            {
              case "begin":
                arrayList1.Add(obj);
                continue;
              case "end":
                arrayList2.Add(obj);
                continue;
              default:
                arrayList5.Add(obj);
                continue;
            }
          }
          else
            arrayList5.Add(obj);
        }
        else if (obj is CodeStatementCollection)
        {
          foreach (CodeStatement codeStatement in (CollectionBase) obj)
          {
            if (codeStatement is CodeAssignStatement && ((CodeAssignStatement) codeStatement).Left is CodeFieldReferenceExpression)
              arrayList4.Add((object) codeStatement);
            else if (codeStatement is CodeVariableDeclarationStatement)
            {
              arrayList3.Add((object) codeStatement);
            }
            else
            {
              string str = codeStatement.UserData[(object) "statement-ordering"] as string;
              if (str != null)
              {
                switch (str)
                {
                  case "begin":
                    arrayList1.Add((object) codeStatement);
                    continue;
                  case "end":
                    arrayList2.Add((object) codeStatement);
                    continue;
                  default:
                    arrayList5.Add((object) codeStatement);
                    continue;
                }
              }
              else
                arrayList5.Add((object) codeStatement);
            }
          }
        }
      }
      statements.AddRange((CodeStatement[]) arrayList3.ToArray(typeof (CodeStatement)));
      statements.AddRange((CodeStatement[]) arrayList4.ToArray(typeof (CodeStatement)));
      statements.AddRange((CodeStatement[]) arrayList1.ToArray(typeof (CodeStatement)));
      statements.AddRange((CodeStatement[]) arrayList5.ToArray(typeof (CodeStatement)));
      statements.AddRange((CodeStatement[]) arrayList2.ToArray(typeof (CodeStatement)));
    }

    private CodeStatementCollection SerializeRootObject(IDesignerSerializationManager manager, object value, bool designTime)
    {
      if ((CodeTypeDeclaration) manager.Context[typeof (CodeTypeDeclaration)] == null)
        return (CodeStatementCollection) null;
      CodeStatementCollection statements = new CodeStatementCollection();
      using (CodeDomSerializerBase.TraceScope("RootCodeDomSerializer::SerializeRootObject"))
      {
        if (designTime)
        {
          this.SerializeProperties(manager, statements, value, RootCodeDomSerializer.designTimeProperties);
        }
        else
        {
          this.SerializeProperties(manager, statements, value, RootCodeDomSerializer.runTimeProperties);
          this.SerializeEvents(manager, statements, value, (Attribute[]) null);
        }
      }
      return statements;
    }

    private class StatementOrderComparer : IComparer
    {
      public static readonly RootCodeDomSerializer.StatementOrderComparer Default;

      static StatementOrderComparer()
      {
        RootCodeDomSerializer.StatementOrderComparer.Default = new RootCodeDomSerializer.StatementOrderComparer();
      }

      private StatementOrderComparer()
      {
        base.\u002Ector();
      }

      public int Compare(object left, object right)
      {
        CodeDomSerializerBase.OrderedCodeStatementCollection statementCollection1 = left as CodeDomSerializerBase.OrderedCodeStatementCollection;
        CodeDomSerializerBase.OrderedCodeStatementCollection statementCollection2 = right as CodeDomSerializerBase.OrderedCodeStatementCollection;
        if (left == null)
          return 1;
        if (right == null)
          return -1;
        if (right == left)
          return 0;
        else
          return statementCollection1.Order - statementCollection2.Order;
      }
    }

    private class ComponentComparer : IComparer
    {
      public static readonly RootCodeDomSerializer.ComponentComparer Default;

      static ComponentComparer()
      {
        RootCodeDomSerializer.ComponentComparer.Default = new RootCodeDomSerializer.ComponentComparer();
      }

      private ComponentComparer()
      {
        base.\u002Ector();
      }

      public int Compare(object left, object right)
      {
        int num = string.Compare(((IComponent) left).GetType().Name, ((IComponent) right).GetType().Name, false, CultureInfo.InvariantCulture);
        if (num == 0)
          num = string.Compare(((IComponent) left).Site.Name, ((IComponent) right).Site.Name, true, CultureInfo.InvariantCulture);
        return num;
      }
    }
  }

    class ComponentCodeDomSerializer : CodeDomSerializer
  {
    private Type[] _containerConstructor;
    private static readonly Attribute[] _runTimeFilter;
    private static readonly Attribute[] _designTimeFilter;
    private static WeakReference _defaultSerializerRef;

    internal static ComponentCodeDomSerializer Default
    {
      get
      {
        if (ComponentCodeDomSerializer._defaultSerializerRef != null)
        {
          ComponentCodeDomSerializer codeDomSerializer = ComponentCodeDomSerializer._defaultSerializerRef.Target as ComponentCodeDomSerializer;
          if (codeDomSerializer != null)
            return codeDomSerializer;
        }
        ComponentCodeDomSerializer codeDomSerializer1 = new ComponentCodeDomSerializer();
        ComponentCodeDomSerializer._defaultSerializerRef = new WeakReference((object) codeDomSerializer1);
        return codeDomSerializer1;
      }
    }

    static ComponentCodeDomSerializer()
    {
      ComponentCodeDomSerializer._runTimeFilter = new Attribute[1]
      {
        (Attribute) DesignOnlyAttribute.No
      };
      ComponentCodeDomSerializer._designTimeFilter = new Attribute[1]
      {
        (Attribute) DesignOnlyAttribute.Yes
      };
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public ComponentCodeDomSerializer()
    {
      base.\u002Ector();
    }

    protected override object DeserializeInstance(IDesignerSerializationManager manager, Type type, object[] parameters, string name, bool addToContainer)
    {
      object obj = base.DeserializeInstance(manager, type, parameters, name, addToContainer);
      if (obj != null)
        this.DeserializePropertiesFromResources(manager, obj, ComponentCodeDomSerializer._designTimeFilter);
      return obj;
    }

    public override object Serialize(IDesignerSerializationManager manager, object value)
    {
      CodeStatementCollection statements = (CodeStatementCollection) null;
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
      using (CodeDomSerializerBase.TraceScope("ComponentCodeDomSerializer::Serialize"))
      {
        if (manager == null || value == null)
          throw new ArgumentNullException(manager == null ? "manager" : "value");
        if (this.IsSerialized(manager, value))
          return (object) this.GetExpression(manager, value);
        InheritanceLevel inheritanceLevel = InheritanceLevel.NotInherited;
        InheritanceAttribute inheritanceAttribute = (InheritanceAttribute) TypeDescriptor.GetAttributes(value)[typeof (InheritanceAttribute)];
        if (inheritanceAttribute != null)
          inheritanceLevel = inheritanceAttribute.InheritanceLevel;
        if (inheritanceLevel != InheritanceLevel.InheritedReadOnly)
        {
          statements = new CodeStatementCollection();
          CodeTypeDeclaration codeTypeDeclaration = manager.Context[typeof (CodeTypeDeclaration)] as CodeTypeDeclaration;
          RootContext rootContext = manager.Context[typeof (RootContext)] as RootContext;
          bool flag1 = false;
          bool flag2 = true;
          bool flag3 = true;
          bool flag4 = false;
          CodeExpression codeExpression = this.GetExpression(manager, value);
          if (codeExpression != null)
          {
            flag1 = false;
            flag2 = false;
            flag3 = false;
            IComponent component = value as IComponent;
            if (component != null && component.Site == null)
            {
              ExpressionContext expressionContext = manager.Context[typeof (ExpressionContext)] as ExpressionContext;
              if (expressionContext == null || expressionContext.PresetValue != value)
                flag4 = true;
            }
          }
          else
          {
            if (inheritanceLevel == InheritanceLevel.NotInherited)
            {
              PropertyDescriptor propertyDescriptor = properties["GenerateMember"];
              if (propertyDescriptor != null && propertyDescriptor.PropertyType == typeof (bool) && !(bool) propertyDescriptor.GetValue(value))
              {
                flag1 = true;
                flag2 = false;
              }
            }
            else
              flag3 = false;
            if (rootContext == null)
            {
              flag1 = true;
              flag2 = false;
            }
          }
          manager.Context.Push(value);
          manager.Context.Push((object) statements);
          try
          {
            string name = manager.GetName(value);
            string className = TypeDescriptor.GetClassName(value);
            if ((flag2 || flag1) && name != null)
            {
              if (flag2)
              {
                if (inheritanceLevel == InheritanceLevel.NotInherited)
                {
                  CodeMemberField codeMemberField = new CodeMemberField(className, name);
                  PropertyDescriptor propertyDescriptor = properties["Modifiers"] ?? properties["DefaultModifiers"];
                  MemberAttributes memberAttributes = propertyDescriptor == null || !(propertyDescriptor.PropertyType == typeof (MemberAttributes)) ? MemberAttributes.Private : (MemberAttributes) propertyDescriptor.GetValue(value);
                  codeMemberField.Attributes = memberAttributes;
                  codeTypeDeclaration.Members.Add((CodeTypeMember) codeMemberField);
                }
                codeExpression = (CodeExpression) new CodeFieldReferenceExpression(rootContext.Expression, name);
              }
              else
              {
                if (inheritanceLevel == InheritanceLevel.NotInherited)
                {
                  CodeVariableDeclarationStatement declarationStatement = new CodeVariableDeclarationStatement(className, name);
                  statements.Add((CodeStatement) declarationStatement);
                }
                codeExpression = (CodeExpression) new CodeVariableReferenceExpression(name);
              }
            }
            if (flag3)
            {
              IContainer container = manager.GetService(typeof (IContainer)) as IContainer;
              ConstructorInfo constructorInfo = (ConstructorInfo) null;
              if (container != null)
                constructorInfo = CodeDomSerializerBase.GetReflectionTypeHelper(manager, value).GetConstructor(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.ExactBinding, (Binder) null, this.GetContainerConstructor(manager), (ParameterModifier[]) null);
              CodeExpression right;
              if (constructorInfo != (ConstructorInfo) null)
              {
                right = (CodeExpression) new CodeObjectCreateExpression(className, new CodeExpression[1]
                {
                  this.SerializeToExpression(manager, (object) container)
                });
              }
              else
              {
                bool isComplete;
                right = this.SerializeCreationExpression(manager, value, out isComplete);
              }
              if (right != null)
              {
                if (codeExpression == null)
                {
                  if (flag4)
                    codeExpression = right;
                }
                else
                {
                  CodeAssignStatement codeAssignStatement = new CodeAssignStatement(codeExpression, right);
                  statements.Add((CodeStatement) codeAssignStatement);
                }
              }
            }
            if (codeExpression != null)
              this.SetExpression(manager, value, codeExpression);
            if (codeExpression != null)
            {
              if (!flag4)
              {
                bool flag5 = value is ISupportInitialize;
                if (flag5)
                {
                  string fullName = typeof (ISupportInitialize).FullName;
                  flag5 = manager.GetType(fullName) != (Type) null;
                }
                Type c1 = (Type) null;
                if (flag5)
                {
                  c1 = CodeDomSerializerBase.GetReflectionTypeHelper(manager, value);
                  flag5 = CodeDomSerializerBase.GetReflectionTypeFromTypeHelper(manager, typeof (ISupportInitialize)).IsAssignableFrom(c1);
                }
                bool flag6 = value is IPersistComponentSettings && ((IPersistComponentSettings) value).SaveSettings;
                if (flag6)
                {
                  string fullName = typeof (IPersistComponentSettings).FullName;
                  flag6 = manager.GetType(fullName) != (Type) null;
                }
                if (flag6)
                {
                  Type c2 = c1 ?? CodeDomSerializerBase.GetReflectionTypeHelper(manager, value);
                  flag6 = CodeDomSerializerBase.GetReflectionTypeFromTypeHelper(manager, typeof (IPersistComponentSettings)).IsAssignableFrom(c2);
                }
                IDesignerSerializationManager serializationManager = (IDesignerSerializationManager) manager.GetService(typeof (IDesignerSerializationManager));
                if (flag5)
                  this.SerializeSupportInitialize(manager, statements, codeExpression, value, "BeginInit");
                this.SerializePropertiesToResources(manager, statements, value, ComponentCodeDomSerializer._designTimeFilter);
                ComponentCache cache = (ComponentCache) manager.GetService(typeof (ComponentCache));
                ComponentCache.Entry entry = (ComponentCache.Entry) null;
                if (cache == null)
                {
                  IServiceContainer serviceContainer = (IServiceContainer) manager.GetService(typeof (IServiceContainer));
                  if (serviceContainer != null)
                  {
                    cache = new ComponentCache(manager);
                    serviceContainer.AddService(typeof (ComponentCache), (object) cache);
                  }
                }
                else if (manager == serializationManager && cache != null && cache.Enabled)
                  entry = cache[value];
                if (entry == null || entry.Tracking)
                {
                  if (entry == null)
                  {
                    entry = new ComponentCache.Entry(cache);
                    ComponentCache.Entry entryAll = cache.GetEntryAll(value);
                    if (entryAll != null && entryAll.Dependencies != null && entryAll.Dependencies.Count > 0)
                    {
                      foreach (object dep in entryAll.Dependencies)
                        entry.AddDependency(dep);
                    }
                  }
                  entry.Component = value;
                  bool flag7 = manager == serializationManager;
                  entry.Valid = flag7 && this.CanCacheComponent(manager, value, properties);
                  if (flag7)
                  {
                    if (cache != null)
                    {
                      if (cache.Enabled)
                      {
                        manager.Context.Push((object) cache);
                        manager.Context.Push((object) entry);
                      }
                    }
                  }
                  try
                  {
                    entry.Statements = new CodeStatementCollection();
                    this.SerializeProperties(manager, entry.Statements, value, ComponentCodeDomSerializer._runTimeFilter);
                    this.SerializeEvents(manager, entry.Statements, value, (Attribute[]) null);
                    foreach (CodeStatement codeStatement in (CollectionBase) entry.Statements)
                    {
                      if (codeStatement is CodeVariableDeclarationStatement)
                      {
                        entry.Tracking = true;
                        break;
                      }
                    }
                    if (entry.Statements.Count > 0)
                    {
                      entry.Statements.Insert(0, (CodeStatement) new CodeCommentStatement(string.Empty));
                      entry.Statements.Insert(0, (CodeStatement) new CodeCommentStatement(name));
                      entry.Statements.Insert(0, (CodeStatement) new CodeCommentStatement(string.Empty));
                      if (flag7)
                      {
                        if (cache != null)
                        {
                          if (cache.Enabled)
                            cache[value] = entry;
                        }
                      }
                    }
                  }
                  finally
                  {
                    if (flag7 && cache != null && cache.Enabled)
                    {
                      manager.Context.Pop();
                      manager.Context.Pop();
                    }
                  }
                }
                else if ((entry.Resources != null || entry.Metadata != null) && (cache != null && cache.Enabled))
                  ResourceCodeDomSerializer.Default.ApplyCacheEntry(manager, entry);
                statements.AddRange(entry.Statements);
                if (flag6)
                  this.SerializeLoadComponentSettings(manager, statements, codeExpression, value);
                if (flag5)
                  this.SerializeSupportInitialize(manager, statements, codeExpression, value, "EndInit");
              }
            }
          }
          catch (CheckoutException ex)
          {
            throw;
          }
          catch (Exception ex)
          {
            manager.ReportError((object) ex);
          }
          finally
          {
            manager.Context.Pop();
            manager.Context.Pop();
          }
        }
      }
      return (object) statements;
    }

    private Type[] GetContainerConstructor(IDesignerSerializationManager manager)
    {
      if (this._containerConstructor == null)
        this._containerConstructor = new Type[1]
        {
          CodeDomSerializerBase.GetReflectionTypeFromTypeHelper(manager, typeof (IContainer))
        };
      return this._containerConstructor;
    }

    private bool CanCacheComponent(IDesignerSerializationManager manager, object value, PropertyDescriptorCollection props)
    {
      IComponent component = value as IComponent;
      if (component != null)
      {
        if (component.Site != null)
        {
          INestedSite nestedSite = component.Site as INestedSite;
          if (nestedSite != null && !string.IsNullOrEmpty(nestedSite.FullName))
            return false;
        }
        if (props == null)
          props = TypeDescriptor.GetProperties((object) component);
        foreach (PropertyDescriptor propertyDescriptor in props)
        {
          if (typeof (IComponent).IsAssignableFrom(propertyDescriptor.PropertyType) && !propertyDescriptor.Attributes.Contains((Attribute) DesignerSerializationVisibilityAttribute.Hidden))
          {
            MemberCodeDomSerializer codeDomSerializer = (MemberCodeDomSerializer) manager.GetSerializer(propertyDescriptor.GetType(), typeof (MemberCodeDomSerializer));
            if (codeDomSerializer != null && codeDomSerializer.ShouldSerialize(manager, value, (MemberDescriptor) propertyDescriptor))
              return false;
          }
        }
      }
      return true;
    }

    private void SerializeLoadComponentSettings(IDesignerSerializationManager manager, CodeStatementCollection statements, CodeExpression valueExpression, object value)
    {
      CodeMethodReferenceExpression referenceExpression = new CodeMethodReferenceExpression((CodeExpression) new CodeCastExpression(new CodeTypeReference(typeof (IPersistComponentSettings)), valueExpression), "LoadComponentSettings");
      CodeMethodInvokeExpression invokeExpression = new CodeMethodInvokeExpression();
      invokeExpression.Method = referenceExpression;
      CodeExpressionStatement expressionStatement = new CodeExpressionStatement((CodeExpression) invokeExpression);
      expressionStatement.UserData[(object) "statement-ordering"] = (object) "end";
      statements.Add((CodeStatement) expressionStatement);
    }

    private void SerializeSupportInitialize(IDesignerSerializationManager manager, CodeStatementCollection statements, CodeExpression valueExpression, object value, string methodName)
    {
      CodeMethodReferenceExpression referenceExpression = new CodeMethodReferenceExpression((CodeExpression) new CodeCastExpression(new CodeTypeReference(typeof (ISupportInitialize)), valueExpression), methodName);
      CodeMethodInvokeExpression invokeExpression = new CodeMethodInvokeExpression();
      invokeExpression.Method = referenceExpression;
      CodeExpressionStatement expressionStatement = new CodeExpressionStatement((CodeExpression) invokeExpression);
      if (methodName == "BeginInit")
        expressionStatement.UserData[(object) "statement-ordering"] = (object) "begin";
      else
        expressionStatement.UserData[(object) "statement-ordering"] = (object) "end";
      statements.Add((CodeStatement) expressionStatement);
    }
  }

   }
#endif