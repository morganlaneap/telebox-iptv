import { FC, useEffect, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { useNavigate } from "react-router-dom";
import MainLayout from "layouts/MainLayout";
import {
  Button,
  Card,
  CardActions,
  CardContent,
  FormControl,
  Typography,
  TextField,
} from "@mui/material";
import SettingService from "services/SettingService";
import { ISystemSettings, IUpdateSettingRequest } from "models/Setting";

const Settings: FC = () => {
  const queryClient = useQueryClient();
  const settingService = new SettingService();

  const [systemSettings, setSystemSettings] = useState<ISystemSettings>({
    outputPath: "",
  });

  const { data: settings, isLoading } = useQuery(
    `getSettings`,
    settingService.getAll,
    {
      refetchOnWindowFocus: false,
    }
  );

  useEffect(() => {
    if (settings) {
      setSystemSettings({
        outputPath: settings.find((s) => s.name === "outputPath")?.value ?? "",
      });
    }
  }, [settings]);

  const { mutate: updateSettings } = useMutation(
    (request: IUpdateSettingRequest) => settingService.update(request),
    {
      onSuccess: () => {
        queryClient.invalidateQueries("getSettings");
      },
    }
  );

  return (
    <MainLayout>
      <Typography sx={{ mb: 2, flexGrow: 1 }} variant="h5" fontWeight="bold">
        Settings
      </Typography>

      <Card>
        {settings && (
          <>
            <CardContent>
              <FormControl fullWidth>
                <TextField
                  label="Recordings output path"
                  name="outputPath"
                  value={systemSettings.outputPath}
                  onChange={(e) =>
                    setSystemSettings({
                      ...systemSettings,
                      outputPath: e.target.value,
                    })
                  }
                />
              </FormControl>
            </CardContent>
            <CardActions>
              <Button
                onClick={() => {
                  if (settings.find((x) => x.name === "outputPath")) {
                    updateSettings({
                      settings: [
                        ...settings,
                        {
                          ...settings.find((x) => x.name === "outputPath")!,
                          value: systemSettings.outputPath,
                        },
                      ],
                    });
                  } else {
                    updateSettings({
                      settings: [
                        ...settings,
                        {
                          name: "outputPath",
                          value: systemSettings.outputPath,
                        },
                      ],
                    });
                  }
                }}
              >
                Save Changes
              </Button>
            </CardActions>
          </>
        )}
      </Card>
    </MainLayout>
  );
};

export default Settings;
