<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="eebf67ea-4a4d-455c-b8dc-7062028f6f4f" namespace="ProcessingEndpoint.Config" xmlSchemaNamespace="urn:ProcessingEndpoint.Config" assemblyName="ProcessingEndpoint.Config" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
            <configurationElementMoniker name="/eebf67ea-4a4d-455c-b8dc-7062028f6f4f/FileShareDataBus" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationSection name="CommandBusConfig" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="commandBusConfig">
      <elementProperties>
        <elementProperty name="CommandHandlers" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="commandHandlers" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/eebf67ea-4a4d-455c-b8dc-7062028f6f4f/CommandHandlers" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="FileShareDataBus">
      <attributeProperties>
        <attributeProperty name="Path" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="path" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/eebf67ea-4a4d-455c-b8dc-7062028f6f4f/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="CommandHandlers" collectionType="AddRemoveClearMap" xmlItemName="add" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/eebf67ea-4a4d-455c-b8dc-7062028f6f4f/CommandHandler" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="CommandHandler">
      <attributeProperties>
        <attributeProperty name="Commands" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="commands" isReadOnly="false" browsable="false">
          <type>
            <externalTypeMoniker name="/eebf67ea-4a4d-455c-b8dc-7062028f6f4f/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>