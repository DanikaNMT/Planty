# Development stage
FROM node:20-alpine AS development
WORKDIR /app

# Copy package files
COPY package.json package-lock.json* yarn.lock* ./

# Install dependencies
RUN npm ci || npm install

# Copy source code
COPY . .

# Expose port for Vite dev server
EXPOSE 5173

# Set environment
ENV NODE_ENV=development

# Dev server will be started by docker-compose command
# CMD is defined in docker-compose files

# Build stage
FROM node:20-alpine AS builder
WORKDIR /app

# Copy package files
COPY package.json package-lock.json* yarn.lock* ./

# Install dependencies
RUN npm ci || npm install

# Copy source code
COPY . .

# Accept build argument for API base URL
ARG VITE_API_BASE=http://localhost:5153
ENV VITE_API_BASE=${VITE_API_BASE}

# Build the application
RUN npm run build

# Production stage
FROM node:20-alpine AS production
WORKDIR /app

# Install serve to run the static build
RUN npm install -g serve

# Copy built application from builder
COPY --from=builder /app/dist ./dist

# Expose port
EXPOSE 5173

# Set environment defaults
ENV NODE_ENV=production
ENV VITE_API_URL=http://api:5000

# Start the application (can be overridden by docker-compose)
CMD ["serve", "-s", "dist", "-l", "5173", "--single"]
