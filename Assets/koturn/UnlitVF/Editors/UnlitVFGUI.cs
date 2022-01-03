using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


namespace Koturn.UnlitVF
{
    /// <summary>
    /// Custom editor of UnlitVF.shader
    /// </summary>
    public sealed class UnlitVFGUI : ShaderGUI
    {
        /// <summary>
        /// Property name of "_Cull".
        /// </summary>
        private const string PropNameCull = "_Cull";
        /// <summary>
        /// Property name of "_RenderingMode".
        /// </summary>
        private const string PropNameRenderingMode = "_RenderingMode";
        /// <summary>
        /// Property name of "_AlphaTest".
        /// </summary>
        private const string PropNameAlphaTest = "_AlphaTest";
        /// <summary>
        /// Property name of "_Cutoff".
        /// </summary>
        private const string PropNameCutoff = "_Cutoff";
        /// <summary>
        /// Property name of "_SrcBlend".
        /// </summary>
        private const string PropNameSrcBlend = "_SrcBlend";
        /// <summary>
        /// Property name of "_DstBlend".
        /// </summary>
        private const string PropNameDstBlend = "_DstBlend";
        /// <summary>
        /// Property name of "_SrcBlendAlpha".
        /// </summary>
        private const string PropNameSrcBlendAlpha = "_SrcBlendAlpha";
        /// <summary>
        /// Property name of "_DstBlendAlpha".
        /// </summary>
        private const string PropNameDstBlendAlpha = "_DstBlendAlpha";
        /// <summary>
        /// Property name of "_BlendOp".
        /// </summary>
        private const string PropNameBlendOp = "_BlendOp";
        /// <summary>
        /// Property name of "_BlendOpAlpha".
        /// </summary>
        private const string PropNameBlendOpAlpha = "_BlendOpAlpha";
        /// <summary>
        /// Property name of "_ZTest".
        /// </summary>
        private const string PropNameZTest = "_ZTest";
        /// <summary>
        /// Property name of "_ZWrite".
        /// </summary>
        private const string PropNameZWrite = "_ZWrite";
        /// <summary>
        /// Property name of "_OffsetFact".
        /// </summary>
        private const string PropNameOffsetFact = "_OffsetFact";
        /// <summary>
        /// Property name of "_OffsetUnit".
        /// </summary>
        private const string PropNameOffsetUnit = "_OffsetUnit";
        /// <summary>
        /// Property name of "_ColorMask".
        /// </summary>
        private const string PropNameColorMask = "_ColorMask";
        /// <summary>
        /// Property name of "_AlphaToMask".
        /// </summary>
        private const string PropNameAlphaToMask = "_AlphaToMask";
        /// <summary>
        /// Property name of "_StencilRef".
        /// </summary>
        private const string PropNameStencilRef = "_StencilRef";
        /// <summary>
        /// Property name of "_StencilReadMask".
        /// </summary>
        private const string PropNameStencilReadMask = "_StencilReadMask";
        /// <summary>
        /// Property name of "_StencilWriteMask".
        /// </summary>
        private const string PropNameStencilWriteMask = "_StencilWriteMask";
        /// <summary>
        /// Property name of "_StencilCompFunc".
        /// </summary>
        private const string PropNameStencilCompFunc = "_StencilCompFunc";
        /// <summary>
        /// Property name of "_StencilPass".
        /// </summary>
        private const string PropNameStencilPass = "_StencilPass";
        /// <summary>
        /// Property name of "_StencilFail".
        /// </summary>
        private const string PropNameStencilFail = "_StencilFail";
        /// <summary>
        /// Property name of "_StencilZFail".
        /// </summary>
        private const string PropNameStencilZFail = "_StencilZFail";
        /// <summary>
        /// Property name of "_IgnoreFog".
        /// </summary>
        private const string PropNameIgnoreFog = "_IgnoreFog";
        /// <summary>
        /// Tag name of "RenderType".
        /// </summary>
        private const string TagRenderType = "RenderType";
        /// <summary>
        /// Tag name of "VRCFallback".
        /// </summary>
        private const string TagVRCFallback = "VRCFallback";

        /// <summary>
        /// Keyword of "_AlphaTest" which is enabled.
        /// </summary>
        private static readonly string KeywordAlphaTestOn;


        /// <summary>
        /// Initialize static members.
        /// </summary>
        static UnlitVFGUI()
        {
            KeywordAlphaTestOn = PropNameAlphaTest.ToUpper() + "_ON";
        }


        /// <summary>
        /// Rendering Mode.
        /// </summary>
        private enum RenderingMode
        {
            /// <summary>
            /// Suitable for normal solid objects with no transparent areas.
            /// </summary>
            Opaque,
            /// <summary>
            /// Allows you to create a transparent effect that has hard edges between the opaque and transparent areas.
            /// </summary>
            Cutout,
            /// <summary>
            /// Allows the transparency values to entirely fade an object out, including any specular highlights or reflections it may have.
            /// </summary>
            Fade,
            /// <summary>
            /// Suitable for rendering realistic transparent materials such as clear plastic or glass.
            /// </summary>
            Transparent,
            /// <summary>
            /// Suitable for additive rendering.
            /// </summary>
            Additive,
            /// <summary>
            /// Suitable for multiply rendering.
            /// </summary>
            Multiply,
            /// <summary>
            /// Custom rendering mode.
            /// </summary>
            Custom
        }

        /// <summary>
        /// Shader types of "VRCFallback".
        /// </summary>
        private enum VRCFallbackShaderType
        {
            /// <summary>
            /// Unlit shader.
            /// </summary>
            Unlit,
            /// <summary>
            /// Standard shader.
            /// </summary>
            Standard,
            /// <summary>
            /// VertexLit shader.
            /// </summary>
            VertexLit,
            /// <summary>
            /// Toon shader.
            /// </summary>
            Toon,
            /// <summary>
            /// Particle shader.
            /// </summary>
            Particle,
            /// <summary>
            /// Sprite shader.
            /// </summary>
            Sprite,
            /// <summary>
            /// Matcap shader.
            /// </summary>
            Matcap,
            /// <summary>
            /// MobileToon shader.
            /// </summary>
            MobileToon,
            /// <summary>
            /// Hide the mesh from view, useful for things like raymarching effects.
            /// </summary>
            Hidden
        }

        /// <summary>
        /// Rendering types of "VRCFallback".
        /// </summary>
        private enum VRCFallbackRenderType
        {
            /// <summary>
            /// Same as <see cref="VRCFallbackRenderType.Opaque"/> but no string is added to the tag value.
            /// </summary>
            None,
            /// <summary>
            /// Opaque rendering.
            /// </summary>
            Opaque,
            /// <summary>
            /// Cutout rendering.
            /// </summary>
            Cutout,
            /// <summary>
            /// Transparent rendering.
            /// </summary>
            Transparent,
            /// <summary>
            /// Fade rendering.
            /// </summary>
            Fade
        }

        /// <summary>
        /// Culling types of "VRCFallback".
        /// </summary>
        private enum VRCFallbackCullType
        {
            /// <summary>
            /// Same as <see cref="VRCFallbackCullType.Default"/>.
            /// </summary>
            None,
            /// <summary>
            /// Back face culling.
            /// </summary>
            Default,
            /// <summary>
            /// No culling.
            /// </summary>
            DoubleSided
        }


        /// <summary>
        /// Draw property items.
        /// </summary>
        /// <param name="me">The <see cref="MaterialEditor"/> that are calling this <see cref="OnGUI(MaterialEditor, MaterialProperty[])"/> (the 'owner')</param>
        /// <param name="mps">Material properties of the current selected shader</param>
        public override void OnGUI(MaterialEditor me, MaterialProperty[] mps)
        {
            TexturePropertySingleLine(me, mps, "_MainTex", "_Color");
            TextureWithHdrColor(me, mps, "_EmissionTex", "_EmissionColor");

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Rendering Options", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                ShaderProperty(me, mps, PropNameCull, false);
                DrawRenderingMode(me, mps);
                ShaderProperty(me, mps, PropNameZTest);
                DrawOffsetProperty(me, mps, PropNameOffsetFact, PropNameOffsetUnit);
                ShaderProperty(me, mps, PropNameColorMask, false);
                ShaderProperty(me, mps, PropNameAlphaToMask, false);

                EditorGUILayout.Space();

                DrawBlendProperty(me, mps);

                EditorGUILayout.Space();

                DrawStencilProperty(me, mps);

                EditorGUILayout.Space();

                DrawVRCFallbackGUI(mps.First().targets.Cast<Material>().First());

                EditorGUILayout.Space();

                DrawAdvancedOptions(me, mps);
            }
        }

        /// <summary>
        /// Draw inspector items of <see cref="RenderingMode"/>.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        private void DrawRenderingMode(MaterialEditor me, MaterialProperty[] mps)
        {
            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                var mpRenderingMode = FindProperty(PropNameRenderingMode, mps);
                var mode = (RenderingMode)EditorGUILayout.EnumPopup(mpRenderingMode.displayName, (RenderingMode)mpRenderingMode.floatValue);
                mpRenderingMode.floatValue = (float)mode;

                if (ccScope.changed && mode != RenderingMode.Custom)
                {
                    foreach (var material in mpRenderingMode.targets.Cast<Material>())
                    {
                        ApplyRenderingMode(material, mode);
                    }
                }

                using (new EditorGUI.DisabledScope(mode != RenderingMode.Cutout && mode != RenderingMode.Custom))
                {
                    var mpAlphaTest = FindProperty(PropNameAlphaTest, mps);
                    ShaderProperty(me, mpAlphaTest);
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUI.DisabledScope(mpAlphaTest.floatValue < 0.5))
                    {
                        ShaderProperty(me, mps, PropNameCutoff);
                    }
                }

                using (new EditorGUI.DisabledScope(mode != RenderingMode.Custom))
                {
                    ShaderProperty(me, mps, PropNameZWrite);
                }
            }
        }


        /// <summary>
        /// Change blend of <paramref name="material"/>.
        /// </summary>
        /// <param name="material">Target material</param>
        /// <param name="renderingMode">Rendering mode</param>
        private static void ApplyRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetOverrideTag(TagRenderType, "");
                    material.SetInt(PropNameAlphaTest, 0);
                    material.DisableKeyword(KeywordAlphaTestOn);
                    material.SetInt(PropNameZWrite, 1);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.Zero);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.Zero);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetOverrideTag(TagRenderType, "TransparentCutout");
                    material.SetInt(PropNameAlphaTest, 1);
                    material.EnableKeyword(KeywordAlphaTestOn);
                    material.SetInt(PropNameZWrite, 1);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.Zero);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.Zero);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    material.renderQueue = (int)RenderQueue.AlphaTest;
                    break;
                case RenderingMode.Fade:
                    material.SetOverrideTag(TagRenderType, "Transparent");
                    material.SetInt(PropNameAlphaTest, 0);
                    material.DisableKeyword(KeywordAlphaTestOn);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    material.renderQueue = (int)RenderQueue.Transparent;
                    break;
                case RenderingMode.Transparent:
                    material.SetOverrideTag(TagRenderType, "Transparent");
                    material.SetInt(PropNameAlphaTest, 0);
                    material.DisableKeyword(KeywordAlphaTestOn);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    material.renderQueue = (int)RenderQueue.Transparent;
                    break;
                case RenderingMode.Additive:
                    material.SetOverrideTag(TagRenderType, "Transparent");
                    material.SetInt(PropNameAlphaTest, 0);
                    material.DisableKeyword(KeywordAlphaTestOn);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.One);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    material.renderQueue = (int)RenderQueue.Transparent;
                    break;
                case RenderingMode.Multiply:
                    material.SetOverrideTag(TagRenderType, "Transparent");
                    material.SetInt(PropNameAlphaTest, 0);
                    material.DisableKeyword(KeywordAlphaTestOn);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.DstColor);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.Zero);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.DstColor);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.Zero);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    material.renderQueue = (int)RenderQueue.Transparent;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderingMode), renderingMode, null);
            }
        }

        /// <summary>
        /// Draw inspector items of "Blend".
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        private void DrawBlendProperty(MaterialEditor me, MaterialProperty[] mps)
        {
            var mpRenderingMode = FindProperty(PropNameRenderingMode, mps);

            using (new EditorGUI.DisabledScope((RenderingMode)mpRenderingMode.floatValue != RenderingMode.Custom))
            {
                var propSrcBlend = FindProperty(PropNameSrcBlend, mps, false);
                var propDstBlend = FindProperty(PropNameDstBlend, mps, false);
                if (propSrcBlend == null || propDstBlend == null)
                {
                    return;
                }
                GUILayout.Label("Blend", EditorStyles.boldLabel);
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    ShaderProperty(me, propSrcBlend);
                    ShaderProperty(me, propDstBlend);
                    var propSrcBlendAlpha = FindProperty(PropNameSrcBlendAlpha, mps, false);
                    var propDstBlendAlpha = FindProperty(PropNameDstBlendAlpha, mps, false);
                    if (propSrcBlendAlpha != null || propDstBlendAlpha != null)
                    {
                        ShaderProperty(me, propSrcBlendAlpha);
                        ShaderProperty(me, propDstBlendAlpha);
                    }

                    var propBlendOp = FindProperty(PropNameBlendOp, mps, false);
                    if (propBlendOp == null) {
                        return;
                    }
                    ShaderProperty(me, propBlendOp);

                    var propBlendOpAlpha = FindProperty(PropNameBlendOpAlpha, mps, false);
                    if (propBlendOpAlpha == null) {
                        return;
                    }
                    ShaderProperty(me, propBlendOpAlpha);
                }

            }
        }

        /// <summary>
        /// Draw inspector items of "Offset".
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        /// <param name="propNameFactor">Property name for the first argument of "Offset"</param>
        /// <param name="propNameUnit">Property name for the second argument of "Offset"</param>
        private void DrawOffsetProperty(MaterialEditor me, MaterialProperty[] mps, string propNameFactor, string propNameUnit)
        {
            var propFactor = FindProperty(propNameFactor, mps, false);
            var propUnit = FindProperty(propNameUnit, mps, false);
            if (propFactor == null || propUnit == null)
            {
                return;
            }
            GUILayout.Label("Offset");
            using (new EditorGUI.IndentLevelScope())
            {
                ShaderProperty(me, propFactor);
                ShaderProperty(me, propUnit);
            }
        }

        /// <summary>
        /// Draw inspector items of Stencil.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        private void DrawStencilProperty(MaterialEditor me, MaterialProperty[] mps)
        {
            EditorGUILayout.LabelField("Stencil", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                ShaderProperty(me, mps, PropNameStencilRef);
                ShaderProperty(me, mps, PropNameStencilReadMask);
                ShaderProperty(me, mps, PropNameStencilWriteMask);
                ShaderProperty(me, mps, PropNameStencilCompFunc);
                ShaderProperty(me, mps, PropNameStencilPass);
                ShaderProperty(me, mps, PropNameStencilFail);
                ShaderProperty(me, mps, PropNameStencilZFail);
            }
        }


        /// <summary>
        /// Draw items for the tag, "VRCFallback".
        /// </summary>
        /// <param name="material">A material.</param>
        [Conditional("VRC_SDK_VRCSDK2"), Conditional("VRC_SDK_VRCSDK3")]
        private void DrawVRCFallbackGUI(Material material)
        {
            EditorGUILayout.LabelField(TagVRCFallback, EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                if (GUILayout.Button("Reset to Default"))
                {
                    material.SetOverrideTag(TagVRCFallback, "");
                }
                var tagVal = material.GetTag(TagVRCFallback, false);

                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                using (var ccScope = new EditorGUI.ChangeCheckScope())
                {
                    var shaderType = VRCFallbackPopupItem<VRCFallbackShaderType>("Shader Type", tagVal);
                    var strHidden = VRCFallbackShaderType.Hidden.ToString();
                    var isHidden = shaderType == strHidden;
                    using (new EditorGUI.DisabledScope(isHidden))
                    {
                        var renderingMode = VRCFallbackPopupItem<VRCFallbackRenderType>("Rendering Mode", tagVal, false);
                        var facing = VRCFallbackPopupItem<VRCFallbackCullType>("Facing", tagVal, false);
                        if (ccScope.changed)
                        {
                            var newTagVal = isHidden ? strHidden : new StringBuilder()
                                .Append(shaderType)
                                .Append(renderingMode)
                                .Append(facing)
                                .ToString();
                            EditorGUILayout.LabelField("Result", '"' + newTagVal + '"');
                            material.SetOverrideTag(TagVRCFallback, newTagVal);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Result", '"' + tagVal + '"');
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw one of popup item of VRCFallback shader.
        /// </summary>
        /// <typeparam name="T"><see cref="VRCFallbackShaderType"/>, <see cref="VRCFallbackRenderType"/> or <see cref="VRCFallbackCullType"/>.</typeparam>
        /// <param name="label">Label text.</param>
        /// <param name="tagVal">Value of "VRCFallback".</param>
        /// <param name="allowDefault">Allow default value or not.</param>
        /// <param name="defaultVal">Default value.</param>
        /// <returns>Return name of estimated selected popup item.
        /// If no popup item is found or <paramref name="allowDefault"/> is true and default value is selected,
        /// returns <see cref="string.Empty"/></returns>
        private static string VRCFallbackPopupItem<T>(string label, string tagVal, bool allowDefault = true, int defaultVal = 0)
            where T : Enum
        {
            var type = typeof(T);
            var names = Enum.GetNames(type);
            var val = GetCurrentSelectedValue<T>(tagVal, names, allowDefault);
            val = EditorGUILayout.Popup(label, val, names);
            if (Enum.IsDefined(type, val) && (allowDefault || val != defaultVal))
            {
                return Enum.GetName(type, val);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Estimate and get the current selected popup item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tagVal">Value of "VRCFallback".</param>
        /// <param name="enumNames">Enum names of <see cref="VRCFallbackShaderType"/>, <see cref="VRCFallbackRenderType"/> or <see cref="VRCFallbackCullType"/>.</param>
        /// <param name="allowDefault">Allow default value or not.</param>
        /// <returns>Enum value of selected item.</returns>
        private static int GetCurrentSelectedValue<T>(string tagVal, string[] enumNames, bool allowDefault = true)
            where T : Enum
        {
            var query = enumNames.Zip(Enum.GetValues(typeof(T)).Cast<int>(), (name, val) => (name, val));
            foreach (var (name, val) in allowDefault ? query : query.Skip(1))
            {
                if (tagVal.Contains(name))
                {
                    return val;
                }
            }

            return default;
        }

        /// <summary>
        /// Draw inspector items of advanced options.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        private static void DrawAdvancedOptions(MaterialEditor me, MaterialProperty[] mps)
        {
            GUILayout.Label("Advanced Options", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                ShaderProperty(me, mps, PropNameIgnoreFog, false);
                me.RenderQueueField();
#if UNITY_5_6_OR_NEWER
                me.EnableInstancingField();
                me.DoubleSidedGIField();
#endif  // UNITY_5_6_OR_NEWER
            }
        }

        /// <summary>
        /// Draw default item of specified shader property.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        /// <param name="propName">Name of shader property</param>
        /// <param name="isMandatory">If <c>true</c> then this method will throw an exception
        /// if a property with <<paramref name="propName"/> was not found.</param>
        private static void ShaderProperty(MaterialEditor me, MaterialProperty[] mps, string propName, bool isMandatory = true)
        {
            var prop = FindProperty(propName, mps, isMandatory);
            if (prop != null) {
                ShaderProperty(me, prop);
            }
        }

        /// <summary>
        /// Draw default item of specified shader property.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mp">Target <see cref="MaterialProperty"/></param>
        private static void ShaderProperty(MaterialEditor me, MaterialProperty mp)
        {
            me.ShaderProperty(mp, mp.displayName);
        }

        /// <summary>
        /// Draw default texture and color pair.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        /// <param name="propNameTex">Name of shader property of texture</param>
        /// <param name="propNameColor">Name of shader property of color</param>
        private static void TexturePropertySingleLine(MaterialEditor me, MaterialProperty[] mps, string propNameTex, string propNameColor)
        {
            TexturePropertySingleLine(
                me,
                FindProperty(propNameTex, mps),
                FindProperty(propNameColor, mps));
        }

        /// <summary>
        /// Draw default texture and color pair.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mpTex">Target <see cref="MaterialProperty"/> of texture</param>
        /// <param name="mpColor">Target <see cref="MaterialProperty"/> of color</param>
        private static void TexturePropertySingleLine(MaterialEditor me, MaterialProperty mpTex, MaterialProperty mpColor)
        {
            me.TexturePropertySingleLine(
                new GUIContent(mpTex.displayName, mpColor.displayName),
                mpTex,
                mpColor);
        }

        /// <summary>
        /// Draw default texture and HDR-color pair.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mps"><see cref="MaterialProperty"/> array</param>
        /// <param name="label">Text label</param>
        /// <param name="propNameTex">Name of shader property of texture</param>
        /// <param name="propNameColor">Name of shader property of color</param>
        private static void TextureWithHdrColor(MaterialEditor me, MaterialProperty[] mps, string propNameTex, string propNameColor)
        {
            TextureWithHdrColor(
                me,
                FindProperty(propNameTex, mps),
                FindProperty(propNameColor, mps));
        }

        /// <summary>
        /// Draw default texture and HDR-color pair.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/></param>
        /// <param name="mpTex">Target <see cref="MaterialProperty"/> of texture</param>
        /// <param name="mpColor">Target <see cref="MaterialProperty"/> of texture</param>
        private static void TextureWithHdrColor(MaterialEditor me, MaterialProperty mpTex, MaterialProperty mpColor)
        {
            me.TexturePropertyWithHDRColor(
                new GUIContent(mpTex.displayName, mpColor.displayName),
                mpTex,
                mpColor,
#if !UNITY_2018_1_OR_NEWER
                new ColorPickerHDRConfig(
                    minBrightness: 0,
                    maxBrightness: 10,
                    minExposureValue: -10,
                    maxExposureValue: 10),
#endif  // !UNITY_2018_1_OR_NEWER
                showAlpha: false);
        }
    }
}
