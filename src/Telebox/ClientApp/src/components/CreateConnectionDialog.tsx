import { FC } from "react";
import { Field, Form, Formik } from "formik";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
} from "@mui/material";
import { TextField } from "formik-mui";
import { useMutation, useQueryClient } from "react-query";
import {
  EmptyCreateConnectionRequest,
  ICreateConnectionRequest,
} from "models/Connection";
import ConnectionService from "services/ConnectionService";

interface IProps {
  open: boolean;
  onClose: () => void;
}

const CreateConnectionDialog: FC<IProps> = ({ open, onClose }) => {
  const queryClient = useQueryClient();
  const connectionService = new ConnectionService();

  const { mutate: createConnection, isLoading } = useMutation(
    (connection: ICreateConnectionRequest) =>
      connectionService.create(connection),
    {
      onSuccess: () => {
        queryClient.invalidateQueries("getAllConnections");
        onClose();
      },
    }
  );

  return (
    <Formik
      initialValues={EmptyCreateConnectionRequest}
      onSubmit={(values) => createConnection(values)}
    >
      {({ submitForm }) => (
        <Form>
          <Dialog open={open} onClose={onClose}>
            <DialogTitle>Create Connection</DialogTitle>
            <DialogContent>
              <FormControl fullWidth sx={{ marginBottom: 2 }}>
                <Field
                  component={TextField}
                  name="name"
                  type="text"
                  label="Connection Name"
                />
              </FormControl>
              <FormControl fullWidth sx={{ marginBottom: 2 }}>
                <Field
                  component={TextField}
                  name="username"
                  type="text"
                  label="Username"
                />
              </FormControl>
              <FormControl fullWidth sx={{ marginBottom: 2 }}>
                <Field
                  component={TextField}
                  name="password"
                  type="text"
                  label="Password"
                />
              </FormControl>
              <FormControl fullWidth sx={{ marginBottom: 2 }}>
                <Field
                  component={TextField}
                  name="serverUrl"
                  type="text"
                  label="Server URL"
                />
              </FormControl>
            </DialogContent>
            <DialogActions>
              <Button disabled={isLoading} onClick={submitForm}>
                Save Changes
              </Button>
            </DialogActions>
          </Dialog>
        </Form>
      )}
    </Formik>
  );
};

export default CreateConnectionDialog;
