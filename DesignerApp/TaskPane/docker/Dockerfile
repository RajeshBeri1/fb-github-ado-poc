FROM nginx:stable-alpine

# Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

COPY ./docker/nginx.conf /etc/nginx/nginx.conf

# Copy dist folder fro build stage to nginx public folder
COPY ./dist/ /usr/share/nginx/html

CMD ["nginx", "-g", "daemon off;"]