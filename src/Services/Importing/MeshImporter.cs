using QM_ImporterAPI.Services.ErrorManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Services.Importing
{
    /// <summary>
    /// Importer for .OBJ extension models.
    /// </summary>
    internal static class MeshImporter
    {
        public static ImportOperationResult<Mesh> ImportMeshFromObj(string assetPath)
        {
            // Ensure the file is an .obj file
            var result = new ImportOperationResult<Mesh>();

            // Ensure file exists
            var fileExists = System.IO.File.Exists(assetPath);
            if (!fileExists)
            {
                result.AddError($"The file at {assetPath} does not exist.");
                return result;
            }

            var isObj = EnsureFileIsObj(assetPath);
            if (!isObj)
            {
                result.AddError($"The file at {assetPath} is not a valid .obj file.");
                return result;
            }

            // Perform import here.
            try
            {
                var contents = File.ReadAllLines(assetPath);
                var mesh = ImportObj(contents);
                if (mesh == null)
                {
                    result.AddError($"Failed to import mesh from {assetPath}. The file may be empty or malformed.");
                    return result;
                }
                result.SetResult(mesh);

            }
            catch (Exception ex)
            {
                result.AddError($"An error occurred while importing the mesh from {assetPath}: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Imports a mesh from the given OBJ file contents.
        /// </summary>
        /// <param name="contents">The contents of the OBJ file.</param>
        /// <returns>The imported mesh, or null if the import failed.</returns>
        private static Mesh ImportObj(string[] contents)
        {
            var verticesCache = new List<Vector3>();
            var verticesNormals = new List<Vector3>();
            var verticesTexture = new List<Vector2>();
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();
            bool hasNormals = false;

            var invariantCulture = CultureInfo.InvariantCulture;

            for (int i = 0; i < contents.Length; i++)
            {
                var text = contents[i].Trim();
                if (text.Length == 0)
                {
                    continue;
                }

                var lineContents = text.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var operationType = lineContents[0];
                Logger.LogDebug($"Processing line {i + 1}: {text}");

                switch (operationType)
                {
                    //case "#":
                    //    if (array2.Length == 5 && array2[1] == "muzzle")
                    //    {
                    //        muzzle = new Vector3(float.Parse(array2[2], invariantCulture), float.Parse(array2[3], invariantCulture), float.Parse(array2[4], invariantCulture));
                    //    }
                    //    break;
                    case "v":
                        verticesCache.Add(new Vector3(float.Parse(lineContents[1], invariantCulture), float.Parse(lineContents[2], invariantCulture), float.Parse(lineContents[3], invariantCulture)));
                        break;
                    case "vn":
                        verticesNormals.Add(new Vector3(float.Parse(lineContents[1], invariantCulture), float.Parse(lineContents[2], invariantCulture), float.Parse(lineContents[3], invariantCulture)));
                        hasNormals = true;
                        break;
                    case "vt":
                        verticesTexture.Add(new Vector2(float.Parse(lineContents[1], invariantCulture), float.Parse(lineContents[2], invariantCulture)));
                        break;
                    case "f":
                        for (int j = 2; j < lineContents.Length - 1; j++)
                        {
                            foreach (var k in new[] { 1, j, j + 1 })
                            {
                                var face = lineContents[k].Split('/');

                                int vi = int.Parse(face[0]);
                                vi = vi < 0 ? verticesCache.Count + vi : vi - 1;
                                vertices.Add(verticesCache[vi]);

                                if (face.Length > 1 && face[1].Length > 0)
                                {
                                    int ti = int.Parse(face[1]);
                                    ti = ti < 0 ? verticesTexture.Count + ti : ti - 1;
                                    uvs.Add(verticesTexture[ti]);
                                }
                                else
                                {
                                    uvs.Add(Vector2.zero);
                                }

                                if (face.Length > 2 && face[2].Length > 0)
                                {
                                    int ni = int.Parse(face[2]);
                                    ni = ni < 0 ? verticesNormals.Count + ni : ni - 1;
                                    normals.Add(verticesNormals[ni]);
                                }
                                else
                                {
                                    normals.Add(Vector3.up);
                                }

                                tris.Add(vertices.Count - 1);
                            }
                        }
                        break;
                    default:
                        Logger.LogDebug($"OBJ Comment? -> {text}");
                        break;
                }
            }

            if (tris.Count == 0)
            {
                return null;
            }

            Mesh val = new Mesh();
            if (vertices.Count > 65535)
            {
                val.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }
            val.SetVertices(vertices);
            val.SetUVs(0, uvs);
            val.SetTriangles(tris, 0);
            if (hasNormals)
            {
                val.SetNormals(normals);
            }
            else
            {
                val.RecalculateNormals();
            }
            val.RecalculateBounds();
            val.RecalculateTangents();

            return val;
        }

        private static bool EnsureFileIsObj(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".obj", StringComparison.OrdinalIgnoreCase);
        }
    }
}
