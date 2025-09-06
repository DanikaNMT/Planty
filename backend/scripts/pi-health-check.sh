#!/bin/bash

PI_IP="192.168.1.100"  # Replace with your Pi's IP
API_URL="http://$PI_IP:5000"

echo "🏥 Plant App Health Check"
echo "========================"

# Check if service is running
echo "📊 Service Status:"
ssh pi@$PI_IP "sudo systemctl is-active plantapp"

# Check API endpoint
echo "🌐 API Health Check:"
if curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/plants" | grep -q "200"; then
    echo "✅ API is responding"
else
    echo "❌ API is not responding"
fi

# Check logs
echo "📝 Recent logs:"
ssh pi@$PI_IP "sudo journalctl -u plantapp --no-pager -n 10"