var cityRepository = {
    Remove: (cityId) => {
        let data = {}
        $.ajax({
            type: 'DELETE',
            url: `${urls['City_Remove']}/${cityId}`,
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (data) {
                let state = data || {}
                console.log("state:", state)

                if (state.isOk == true) {
                    toastr.info(`Delete is complete. ${state.message}`)
                    location.reload()
                } else {
                    toastr.info(`Delete had errors. ${state.message}`)
                }
            },
            dataType: 'json'
        })
    }
}
