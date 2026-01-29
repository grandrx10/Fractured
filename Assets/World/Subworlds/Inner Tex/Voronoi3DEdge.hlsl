#ifndef VORONOI_3D_EDGE_INCLUDED
#define VORONOI_3D_EDGE_INCLUDED

float3 hash3(float3 p)
{
    p = frac(p * 0.3183099 + 0.1);
    p *= 17.0;
    return frac(p.xxy * p.yzz * p.zyx);
}

void Voronoi3D_EdgeDistance_float(
    float3 position,
    float scale,
    out float edgeDistance,
    out float cellID
)
{
    // ---- REQUIRED initialization (fixes the error)
    edgeDistance = 0.0;
    cellID = 0.0;

    position *= scale;

    float3 cell = floor(position);
    float3 local = frac(position);

    float d1 = 1e10;
    float d2 = 1e10;
    float id = 0.0;

    [unroll]
    for (int z = -1; z <= 1; z++)
    {
        [unroll]
        for (int y = -1; y <= 1; y++)
        {
            [unroll]
            for (int x = -1; x <= 1; x++)
            {
                float3 offset = float3(x, y, z);
                float3 h = hash3(cell + offset);
                float3 site = offset + h;

                float d = distance(local, site);

                if (d < d1)
                {
                    d2 = d1;
                    d1 = d;
                    id = dot(cell + offset, float3(1.0, 57.0, 113.0));
                }
                else if (d < d2)
                {
                    d2 = d;
                }
            }
        }
    }

    edgeDistance = (d2 - d1) * 0.5;
    cellID = id;
}

#endif
