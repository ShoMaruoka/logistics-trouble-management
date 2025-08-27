import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: 'export',
  trailingSlash: true,
  
  // 本番環境のみベースパスを設定（開発環境は不要）
  basePath: process.env.NODE_ENV === 'production' 
    ? process.env.NEXT_PUBLIC_APP_BASE_PATH || '/ltm'
    : '',
  assetPrefix: process.env.NODE_ENV === 'production' 
    ? process.env.NEXT_PUBLIC_APP_BASE_PATH || '/ltm'
    : '',
  
  images: {
    unoptimized: true
  },
  
  experimental: {
    forceSwcTransforms: true
  },
  
  // 環境変数をそのまま公開
  env: {
    NEXT_PUBLIC_APP_BASE_PATH: process.env.NEXT_PUBLIC_APP_BASE_PATH,
    NEXT_PUBLIC_APP_URL: process.env.NEXT_PUBLIC_APP_URL,
    NEXT_PUBLIC_API_BASE_PATH: process.env.NEXT_PUBLIC_API_BASE_PATH,
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL,
    NEXT_PUBLIC_ENVIRONMENT: process.env.NEXT_PUBLIC_ENVIRONMENT,
    NEXT_PUBLIC_DEBUG_MODE: process.env.NEXT_PUBLIC_DEBUG_MODE,
    NEXT_PUBLIC_LOG_LEVEL: process.env.NEXT_PUBLIC_LOG_LEVEL
  }
};

export default nextConfig;
