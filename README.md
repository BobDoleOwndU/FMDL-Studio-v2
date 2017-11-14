# FMDL Studio v2
Fox Engine model importer (and planned exporter) for Unity.

## Usage

### Importing an fmdl to Unity

Open the project in Unity, go to the *FMDL Studio* menu option and select *Import Fmdl*. Select the fmdl of your choice and it'll be imported. If you want textures to be applied to the model, they must be placed inside a folder with the same name as the model, inside of the Fmdl Studio folder (the folder containing the textures should be in the folder with the Fmdl Studio solution file).

**Note:** Currently only diffuse and normal textures are applied automatically. The tool will look for the other textures used by the model as well; but it will not apply them to the model.

**Note 2:** You must convert the normal maps from the Fox Engine format to the standard normal map format if you want them to look right. To do this you must:

1. Copy the texture's alpha channel to the red channel.

2. Invert the texture's green channel.

3. Paint the blue channel completely white.

4. Paint the alpha channel completely white.

### Exporting a model to FBX

After a model has be imported into Unity, select the model in the editor (the object with the model's name). Go to the *FMDL Studio* menu option and select *Convert to FBX*. Select where you want the model to be exported to and click *Save*.

By default the fbx file will search for textures in a folder with the name of the fmdl file at the same location the fbx was exported to.

**Note:** Currently only diffuse and normal textures are applied automatically.

## Credits
BobDoleOwndU: Programming and reverse-engineering.

Joey35233: Programming.

youarebritish (sai): General help/debugging.

Tex: Testing.

Highflex: Reverse-engineering.

revel8n: Reverse-engineering.

Thanks to Jayveerk, cra0kalo and HeartlessSeph for their previous work on the .fmdl format.
