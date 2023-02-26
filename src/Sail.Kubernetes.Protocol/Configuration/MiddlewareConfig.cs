﻿namespace Sail.Kubernetes.Protocol.Configuration;

public class MiddlewareConfig
{
    public JwtBearerConfig JwtBearer { get; set; }
    public CorsConfig Cors { get; set; }
    public RateLimiterConfig RateLimiter { get; set; }
}