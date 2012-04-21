<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="cfbdd8bf-4c40-40b3-98b6-d81dba5effcc" namespace="ViewModelEndpoint.Config" xmlSchemaNamespace="urn:ViewModelEndpoint.Config" assemblyName="ViewModelEndpoint.Config" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
            <configurationElementMoniker name="/cfbdd8bf-4c40-40b3-98b6-d81dba5effcc/FileShareDataBus" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationSection name="DomainEventBusConfig" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="domainEventBusConfig">
      <elementProperties>
        <elementProperty name="DomainEventEndpointMappings" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="domainEventEndpointMappings" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/cfbdd8bf-4c40-40b3-98b6-d81dba5effcc/DomainEventEndpointMappings" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="FileShareDataBus">
      <attributeProperties>
        <attributeProperty name="Path" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="path" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/cfbdd8bf-4c40-40b3-98b6-d81dba5effcc/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="DomainEventEndpointMappings" collectionType="AddRemoveClearMap" xmlItemName="add" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/cfbdd8bf-4c40-40b3-98b6-d81dba5effcc/DomainEventEndpointMapping" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="DomainEventEndpointMapping">
      <attributeProperties>
        <attributeProperty name="DomainEvents" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="domainEvents" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/cfbdd8bf-4c40-40b3-98b6-d81dba5effcc/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Endpoint" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="endpoint" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/cfbdd8bf-4c40-40b3-98b6-d81dba5effcc/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>