<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="de1e696b-5e67-4901-b99f-9472610d5be0" namespace="CommandEndpoint.Config" xmlSchemaNamespace="urn:CommandEndpoint.Config" assemblyName="CommandEndpoint.Config" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="DataBusConfig" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="dataBusConfig">
      <elementProperties>
        <elementProperty name="FileShareDataBus" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="fileShareDataBus" isReadOnly="false" browsable="false">
          <type>
            <configurationElementMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/FileShareDataBus" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationSection name="CommandBusConfig" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="commandBusConfig">
      <elementProperties>
        <elementProperty name="CommandEndpointMappings" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="commandEndpointMappings" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/CommandEndpointMappings" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="FileShareDataBus">
      <attributeProperties>
        <attributeProperty name="Path" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="path" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="CommandEndpointMapping">
      <attributeProperties>
        <attributeProperty name="Commands" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="commands" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Endpoint" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="endpoint" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="CommandEndpointMappings" collectionType="AddRemoveClearMap" xmlItemName="add" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/CommandEndpointMapping" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="Certificates">
      <elementProperties>
        <elementProperty name="ClaimsSigningCertificate" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="claimsSigningCertificate" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/ClaimsSigningCertificate" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="ClaimsSigningCertificate">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="name" isReadOnly="false" browsable="false">
          <type>
            <externalTypeMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationSection name="SecurityConfig" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="securityConfig">
      <elementProperties>
        <elementProperty name="Certificates" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="certificates" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/de1e696b-5e67-4901-b99f-9472610d5be0/Certificates" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>